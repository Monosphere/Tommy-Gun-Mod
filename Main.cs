using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace TommyGunMod
{
    public class Main : MelonMod
    {
        private string WeaponParentPath = "Player_Audrey(Clone)/Content/CameraPivot/HeadContainer/CameraContainer/GameCamera/ArmsContainer/Audrey@TPose/BN_Root001/BN_Pelvis001/BN_Spine_A001/BN_Spine_B001/BN_Spine_C001/BN_Spine_D001/BN_Chest001/BN_R_Clavicle001/BN_R_UpperArm001/BN_R_ForeArm001/BN_R_Hand001/BN_R_Prop/WeaponParent/";
        Player player = null;
        GunHandler gunHandler = null;
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            base.OnSceneWasLoaded(buildIndex, sceneName);
            if (sceneName == "InitializeGame")
            {
                if (gunHandler == null)
                {
                    Debug.Log("Slayification");
                    GameObject obj = new GameObject("GunManager");
                    gunHandler = obj.AddComponent<GunHandler>();
                }
            }
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
    public class GunHandler : MonoBehaviour
    {
        private string WeaponParentPath = "Player_Audrey(Clone)/Content/CameraPivot/HeadContainer/CameraContainer/GameCamera/ArmsContainer/";
        Player player = null;
        GameObject tommyGun;
        bool hasGunEquipped = false;
        float timeSinceLastShot;
        RaycastHit[] hits;
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        void Update()
        {
            if (player == null)
            {
                if (GameObject.Find("Player_Audrey(Clone)") != null)
                {
                    player = FindObjectOfType<Player>();
                    Debug.Log("FOUND PLAYER");
                    Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("TommyGunMod.Assets.tommygun");
                    AssetBundle gunAsset = AssetBundle.LoadFromStream(str);

                    tommyGun = Instantiate(gunAsset.LoadAsset<GameObject>("Tommy_Gun"));
                    tommyGun.transform.SetParent(GameObject.Find(WeaponParentPath).transform);
                    tommyGun.transform.localScale = new Vector3(100f, 100f, 100f);
                    tommyGun.transform.localPosition = new Vector3(0.4f, -0.47f, 1.8f);
                    tommyGun.transform.localEulerAngles = Vector3.zero;
                    Debug.Log("Loaded Tommy Gun");
                }
            }
            if (player != null)
            {
                timeSinceLastShot += Time.deltaTime;
                if(player.AnimatorBody.GetInteger("HandWeaponStatus") == 0)
                {
                    tommyGun.SetActive(true);
                }
                else if(player.AnimatorBody.GetInteger("HandWeaponStatus") == 1)
                {
                    tommyGun.SetActive(false);
                }
                if (hasGunEquipped)
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (timeSinceLastShot > 0.08f)
                        {
                            tommyGun.transform.GetChild(0).gameObject.SetActive(!tommyGun.transform.GetChild(0).gameObject.activeSelf);
                            timeSinceLastShot = 0f;
                            if (tommyGun.transform.GetChild(0).gameObject.activeSelf)
                            {
                                tommyGun.GetComponent<AudioSource>().Stop();
                                tommyGun.GetComponent<AudioSource>().Play();
                                hits = Physics.RaycastAll(player.GameCamera.transform.position, player.GameCamera.transform.forward);
                                foreach (RaycastHit hit in hits)
                                {
                                    Transform objectHit = hit.transform;
                                    Debug.Log(objectHit.gameObject.name);
                                    if (objectHit.GetComponent<Enemy>() != null)
                                    {
                                        objectHit.GetComponent<Enemy>().OnHit(new RaycastHit());
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (timeSinceLastShot > 0.08f)
                            tommyGun.transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    Debug.Log("Swapping Weapons");
                    if (hasGunEquipped)
                    {
                        player.ShowWeapon();
                        //tommyGun.SetActive(false);
                    }
                    else
                    {
                        player.HideWeapon();
                        //tommyGun.SetActive(true);
                    }
                    hasGunEquipped = !hasGunEquipped;
                }
            }
        }
    }
}
