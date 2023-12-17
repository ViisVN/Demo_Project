using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using QFSW.QC;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Mono.CSharp;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviourPun
{
    public Transform viewPoint;
    public float moveSensitivity = 1f;
    private float verticalRotStore;
    private Vector2 moveInput;

    public bool invertLook;

    public float moveSpeed = 5f, runSpeed = 8f;
    private float activeMoveSpeed;

    private Vector3 moveDirection, movement;

    public CharacterController characterController;

    //Gán camera
    private new Camera camera;
    public Camera secondaryCamera;

    public float jumpForce = 12, gravityMode = 2.5f;

    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundLayers;


    public GameObject bulletImpact_wall, bulletImpact_player, death_animtions;

    //public float timeBetweenShots = 0.1f;
    private float shotCounter;
    public float muzzleDisplayTime;
    private float muzzleCounter;

    public float maxHeat = 10f, /*heatPerShot = 1f,*/ coolRate = 4f, overheatCoolRate = 5;
    private float heatCounter;
    private bool overHeated;

    public Gun[] allGuns;
    private int selectedGun;

    private PhotonView view;

    //Thanh máu người chơi
    [SerializeField] public int health = 10, currentHealth = 10;
    [SerializeField] int cur_viewID;

    //Liên quan đến nhận sát thương và trừ máu
    // Biến (Làm cho bất tử)
    [SerializeField] float invinsible_time = 0.5f, based_invinsible_time = 0.5f;
    public bool is_invinsible = false;


    // Biến const byte lệnh raiseevent
    const byte TAKE_DAMAGE = 1, DEATH = 2, REVIVE = 3, CHANGE_COLOR = 4;

    //Team set up
    public bool isTeamBlue = false; //True = BLUE, false = RED
    public bool is_death;
    public Player player = PhotonNetwork.LocalPlayer;
    public int actorNumber;

    //Change Color
    // Start is called before the first frame update

    //Climb Ladder
    private bool isClimbLadder;


    void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += Client_Received_Event;
    }
    void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= Client_Received_Event;
    }
    private void Client_Received_Event(EventData obj)
    {
        if (obj.Code == TAKE_DAMAGE)
        {
            object[] datas = (object[])obj.CustomData;
            int viewID = (int)datas[0];
            if (viewID == view.ViewID)
            {
                if (is_invinsible == false)
                {
                    is_invinsible = true;
                    invinsible_time = 0f;
                    currentHealth -= 1; // Sát thương dự tính
                    Hearts_System.Instance.Player_Create_Heart(currentHealth, health);
                    if (currentHealth == 0)
                    {
                        Debug.Log("Death");
                    }
                }
            }
        }
        //Use when enemy Death
        if (obj.Code == DEATH)
        {
            object[] datas = (object[])obj.CustomData;
            int redScore = (int)datas[0];
            int blueScore = (int)datas[1];
            int viewID = (int)datas[2];
            bool targetIsDeath = (bool)datas[3];
            bool isTeamblue = (bool)datas[4];
            int deathactor = (int)datas[5];
            int killactor = (int)datas[6];
            Score_Manager.Instance.RedScore = redScore;
            Score_Manager.Instance.BlueScore = blueScore;
            Score_Manager.Instance.UpdateScore();
            PlayerList_Manager.instance.playerslist.Find(player => player.actorID == deathactor).death = (int)datas[7];
            PlayerList_Manager.instance.playerslist.Find(player => player.actorID == killactor).kill = (int)datas[8];
            PhotonView.Find(viewID).gameObject.transform.position = SpawnManager.instance.GetSpawnPoints_2(!isTeamblue).position;
        }

    }
    void Start()
    {
        //Get Photon view
        view = GetComponent<PhotonView>();
        Cursor.lockState = CursorLockMode.Locked;

        if (view.IsMine)
        {

            actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            cur_viewID = view.ViewID;
            Debug.Log(cur_viewID);
            camera = Camera.main;
            UIController.instance.tempSlider.maxValue = maxHeat;
            SwithGun();
            Transform newTransform = SpawnManager.instance.GetSpawnPoints();
            transform.position = newTransform.position;
            transform.rotation = newTransform.rotation;
            Hearts_System.Instance.Player_Create_Heart(currentHealth, health);
            PhotonTeamsManager.Instance.TryGetTeamByCode(1, out var BLUE);
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam() == BLUE)
            {
                isTeamBlue = true;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            //Release or lock cursor
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Cursor.lockState == CursorLockMode.None)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            if (!Score_Manager.Instance.gameOver)
            {
                Movement();
                OverHeat_Check_WhenShoot();
                Check_SwitchGun();
                //Take damage
                if (is_invinsible)
                {
                    invinsible_time -= Time.deltaTime;
                    if (invinsible_time <= 0)
                    {
                        is_invinsible = false;
                        invinsible_time = based_invinsible_time;
                    }
                }
            }
        }
    }

    private void Movement()
    {

        moveInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * moveSensitivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + moveInput.x,
            transform.rotation.eulerAngles.z);

        verticalRotStore += moveInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

        if (invertLook)
        {
            viewPoint.rotation = Quaternion.Euler(
            verticalRotStore,
            viewPoint.rotation.eulerAngles.y,
            viewPoint.rotation.eulerAngles.z);
        }
        else
        {
            viewPoint.rotation = Quaternion.Euler(
            -verticalRotStore,
            viewPoint.rotation.eulerAngles.y,
            viewPoint.rotation.eulerAngles.z);
        }

        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));


        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = moveSpeed;
        }
        float yValue = movement.y;
        movement = ((transform.forward * moveDirection.z) + (transform.right * moveDirection.x)).normalized * activeMoveSpeed;
        movement.y = yValue;
        if (characterController.isGrounded)
        {
            movement.y = 0f;
        }

        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayers);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            movement.y = jumpForce;
        }

       

        if (!isClimbLadder)
        {
             movement.y += Physics.gravity.y * Time.deltaTime * gravityMode;
            characterController.Move(movement * Time.deltaTime);
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                characterController.Move(Vector3.up* moveSpeed*20f * Time.deltaTime );
            }
            if (Input.GetKey(KeyCode.S) && !isGrounded)
            {
                characterController.Move(Vector3.down * Time.deltaTime * moveSpeed);
            }
        }


        if (allGuns[selectedGun].muzzleFlash.activeInHierarchy)
        {
            muzzleCounter -= Time.deltaTime;
            if (muzzleCounter <= 0)
            {
                allGuns[selectedGun].muzzleFlash.SetActive(false);
            }
        }
    }

    private void OverHeat_Check_WhenShoot()
    {

        if (!overHeated)
        {
            // shooting
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }

            if (Input.GetMouseButton(0) && allGuns[selectedGun].isAutomatic)
            {
                shotCounter -= Time.deltaTime;
                if (shotCounter <= 0)
                {
                    Shoot();
                }
            }

            heatCounter -= coolRate * Time.deltaTime;
        }
        else
        {
            heatCounter -= overheatCoolRate * Time.deltaTime;
            if (heatCounter <= 0)
            {
                heatCounter = 0;
                overHeated = false;
            }
        }
        if (heatCounter < 0)
        {
            heatCounter = 0f;
        }
        UIController.instance.tempSlider.value = heatCounter;
    }
    private void Shoot()
    {
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        ray.origin = camera.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            switch (hit.collider.gameObject.tag)
            {
                //Trúng tường
                case "Obstacle":
                    Debug.Log(hit.collider.gameObject.name);
                    var one = PhotonNetwork.Instantiate(bulletImpact_wall.name, hit.point + (hit.normal * 0.02f), Quaternion.LookRotation(hit.normal, Vector3.up));
                    var one_view = one.GetComponent<PhotonView>();
                    if (one_view.IsMine)
                    {
                        StartCoroutine(Destroy_Things(one, 5f));
                    }
                    break;

                //Trúng người
                case "Player":
                    var hit_obj = PhotonNetwork.Instantiate(bulletImpact_player.name, hit.point + (hit.normal * 0.02f), Quaternion.LookRotation(hit.normal, Vector3.up));
                    var obj_view = hit_obj.GetComponent<PhotonView>();
                    var hited_view = hit.collider.gameObject.GetComponent<PhotonView>();
                    Debug.Log(player.NickName + "--" + PhotonNetwork.LocalPlayer.NickName);
                    TakeDamage(hited_view.ViewID, isTeamBlue, actorNumber);
                    if (obj_view.IsMine)
                    {
                        StartCoroutine(Destroy_Things(hit_obj, 1f));
                    }
                    break;

            }

        }
        shotCounter = allGuns[selectedGun].timeBetweenShots;

        heatCounter += allGuns[selectedGun].heatPerShot;
        if (heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;
            overHeated = true;

            UIController.instance.overheatedText.gameObject.SetActive(true);
        }
        else
        {
            UIController.instance.overheatedText.gameObject.SetActive(false);
        }

        allGuns[selectedGun].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;
    }


    private void LateUpdate()
    {
        if (view.IsMine)
        {
            camera.transform.SetPositionAndRotation(viewPoint.position, viewPoint.rotation);
        }
    }


    void Check_SwitchGun()
    {
        // chuyển súng
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            selectedGun += 1;
            if (selectedGun >= allGuns.Length)
            {
                selectedGun = 0;
            }
            SwithGun();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            selectedGun--;
            if (selectedGun < 0)
            {
                selectedGun = allGuns.Length - 1;
            }
            SwithGun();
        }

        for (int i = 0; i < allGuns.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                selectedGun = i;
                SwithGun();
            }
        }
    }
    void SwithGun()
    {
        foreach (Gun gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }

        allGuns[selectedGun].gameObject.SetActive(true);
        allGuns[selectedGun].muzzleFlash.SetActive(false);
    }

    IEnumerator Destroy_Things(GameObject obj, float waits_Time)
    {
        yield return new WaitForSecondsRealtime(waits_Time);

        PhotonNetwork.Destroy(obj);
    }

    [Command()]
    void CreateHeart(int _health, int _cur_health)
    {
        health = _health;
        currentHealth = _cur_health;
        Hearts_System.Instance.Player_Create_Heart(currentHealth, health);
    }

    //Nhan sat thuong
    private void TakeDamage(int viewID, bool isTeamBlue, int actorNumber)
    {
        view.RPC(nameof(TakeDamage_RPC), RpcTarget.All, viewID, isTeamBlue, actorNumber);
    }

    [PunRPC]
    private void TakeDamage_RPC(int viewID, bool isTeamBlue, int actorNumber)
    {
        var target = PhotonView.Find(viewID).gameObject.GetComponent<PlayerController>();
        if (PhotonView.Find(viewID).IsMine)
        {
            if (target.isTeamBlue == isTeamBlue)
            {
                Debug.Log("SameTeam");
                return;
            }
            if (!target.is_invinsible && !target.is_death)
            {
                target.currentHealth -= 1;
                target.is_invinsible = true;
                Hearts_System.Instance.Player_Create_Heart(target.currentHealth, target.health);
                if (target.currentHealth <= 0)
                {
                    target.currentHealth = target.health;
                    Hearts_System.Instance.Player_Create_Heart(target.currentHealth, target.health);
                    if (target.isTeamBlue)
                    {
                        Score_Manager.Instance.RedScore += 1;
                    }
                    else
                    {
                        Score_Manager.Instance.BlueScore += 1;
                    }
                    Score_Manager.Instance.UpdateScore();
                    // int - int - int - bool - bool - int - int - int - int
                    PhotonNetwork.Instantiate(death_animtions.name, target.gameObject.transform.position, Quaternion.identity);
                    target.transform.position = SpawnManager.instance.GetSpawnPoints_2(!isTeamBlue).position;
                    int deathnumber = PlayerList_Manager.instance.playerslist.Find(player => player.actorID == target.actorNumber).death += 1;
                    int killnumber = PlayerList_Manager.instance.playerslist.Find(player => player.actorID == actorNumber).kill += 1;
                    object[] datas = new object[]{Score_Manager.Instance.RedScore,Score_Manager.Instance.BlueScore,viewID,target.is_death,target.isTeamBlue,
                    target.actorNumber,actorNumber,deathnumber,killnumber};
                    PhotonNetwork.RaiseEvent(DEATH, datas, RaiseEventOptions.Default, SendOptions.SendUnreliable);
                }
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Ladder"))
        {
            isClimbLadder = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Ladder"))
        {
            isClimbLadder = false;
        }
    }
}
