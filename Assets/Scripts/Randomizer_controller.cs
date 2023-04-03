using System.IO;
using System;
using System.Collections.Generic;
//using System.Web.Script.Serialization;

namespace UnityEngine
{
    public class Randomizer_controller : MonoBehaviour
    {
        private GameObject loadedModel;
        private GameObject dirLight;
        private Object[] modelList;
        private Object[] skyMaterialList;
        private Object[] modelMaterialList;
        private int modelCounter;
        private int skyMaterialCounter;
        private int modelMaterialCounter;
        private RandCameraController mainCameraController;
        private bool isRecording;
    

        [Header("Folder where the dataset will be saved")]
        [Space(5)]
        public string screensFolderName = "";
        [Space(10)]

        [Header("Directional light")]
        [Space(5)]
        [Tooltip("x: min, y: max")]
        public Vector2 intensityLev = new Vector2 (0.5f, 1.5f);
        
        [Space(5)]
        [Tooltip("flag to decide if the directional light color is going to be randomized")]
        public bool isColorRandomized = true;
        [Space(5)]
        [Tooltip("flag to decide if the skybox is going to be randomized")]
        public bool useSkyBoxRandomization = false;
        
        

        // Start is called before the first frame updateprivate static Random rng = new Random();  
        void Start()
        {
            // Time.fixedDeltaTime = 0.01f;
            // Time.maximumDeltaTime = 0.01f;

            isRecording = true;

            if (screensFolderName == "")
                screensFolderName = Application.dataPath + "/Screens/";

            modelCounter = -1;
            skyMaterialCounter = -1;
            modelMaterialCounter = -1;
            modelMaterialCounter = -1;

            modelList = Resources.LoadAll("Models", typeof(GameObject));
            skyMaterialList = Resources.LoadAll("HDRIHaven/Materials", typeof(Material));
            modelMaterialList = Resources.LoadAll("Materials", typeof(Material));

            dirLight = GameObject.Find("Directional Light");

            GameObject temp_randomizer = GameObject.Find("Main Camera");
            mainCameraController = temp_randomizer.GetComponent<RandCameraController>();

            Debug.Log("Randomizer started...");
        }

        void Update()
        {
            randomizerUpdate();
        }
        void LateUpdate()
        {
            if (isRecording)
                saveOnDisk();
        }

        void randomizerUpdate()
        {
            Destroy(loadedModel);
            loadedModel = spawnNextModel();
            // MeshRenderer modelMeshRenderer = loadedModel.GetComponent<MeshRenderer>();
            // modelMeshRenderer.material = spawnNextModelMaterial();

            if (useSkyBoxRandomization)
            {
                RenderSettings.skybox = spawnNextSkyMaterial();
                DynamicGI.UpdateEnvironment();
            }

            dirLight.transform.rotation = Random.rotation;
            Light tempLight = dirLight.GetComponent<Light>();
            tempLight.intensity = Random.Range(intensityLev[0], intensityLev[1]);
            if (isColorRandomized)
                tempLight.color = Random.ColorHSV();
        }

        void saveOnDisk()
        {
            string folderName = screensFolderName + "/" + modelList[modelCounter].name + "/";
            if(Directory.Exists(folderName) == false)
            {
                Directory.CreateDirectory(folderName);
            }

            int dirCounter = 0;
            string tempPath = folderName + dirCounter++.ToString() + ".png";
            while(File.Exists(tempPath))
                tempPath = folderName + dirCounter++.ToString() + ".png";

            byte[] Bytes = mainCameraController.CamCapture();
            File.WriteAllBytes(tempPath, Bytes);
        }

        GameObject spawnNextModel(bool shuffle = true)
        {
            GameObject tempObject;

            if(shuffle)
                modelCounter = Random.Range(0, modelList.Length);
            else
                modelCounter++;

            if(modelCounter >= modelList.Length)
            {
                modelCounter = 0;
            }
            
            tempObject = Instantiate((GameObject)modelList[modelCounter]);      

            return tempObject;
        }
        Material spawnNextSkyMaterial(bool shuffle = true)
        {
            Material skyMat;

            if(shuffle)
                skyMaterialCounter = Random.Range(0, skyMaterialList.Length);
            else
                skyMaterialCounter++;

            if(skyMaterialCounter >= skyMaterialList.Length)
            {
                skyMaterialCounter = 0;
            }

            skyMat = (Material)skyMaterialList[skyMaterialCounter];        

            return skyMat;
        }

        Material spawnNextModelMaterial(bool shuffle = true)
        {
            Material modelMat;

            if(shuffle)
                modelMaterialCounter = Random.Range(0, modelMaterialList.Length);
            else
                modelMaterialCounter++;

            if(modelMaterialCounter >= modelMaterialList.Length)
            {
                modelMaterialCounter = 0;
            }

            modelMat = (Material)modelMaterialList[modelMaterialCounter];    

            return modelMat;
        }

        public GameObject getModel()
        {
            return loadedModel;
        }

        public string getSaveDir()
        {
            if (screensFolderName == "")
                screensFolderName = Application.dataPath + "/Screens/";

            return screensFolderName;
        }

        public void setSaveDir(string newSaveDir)
        {
            screensFolderName = newSaveDir;
        }
    }
}
