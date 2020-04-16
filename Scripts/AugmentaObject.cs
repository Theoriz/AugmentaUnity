//using System;
using System.Collections.Generic;
using UnityEngine;

namespace Augmenta
{
	public class AugmentaObject : MonoBehaviour
	{
        [Header("Object Settings")]
        public AugmentaManager augmentaManager;
        public bool showDebug = false;
        public GameObject debugObject;
        public GameObject debugVelocityPivot;
        public GameObject debugVelocity;
        public GameObject debugOrientationPivot;
        public GameObject debugOrientation;

        [Header("Augmenta Object Values")]
        public int id;
		public int oid;
		public int ageInFrames;
        public float ageInSeconds;
		public Vector2 centroid;
        public Vector2 velocity;
        public float orientation;
		public float depth;
		public Rect boundingRect;
        public float boundingRectRotation;
		public Vector3 highest;
        public float distanceToSensor;
        public float reflectivity;

        public float inactiveTime;

        private Material augmentaObjectMaterialInstance;

        private bool _initialized = false;

        #region MonoBehaviour Functions

        private void OnEnable() {

            _initialized = false;
        }

        void Update() {

            //Initialization
            if (!_initialized)
                Initialize();

            //Update debug state if incoherent
            if (showDebug != debugObject.activeSelf)
                ShowDebug(showDebug);
        }

        void OnDrawGizmos() {

            Gizmos.color = Color.red;
            DrawGizmoCube(GetAugmentaObjectWorldPosition(true),
                          debugObject.transform.rotation, 
                          GetAugmentaObjectWorldScale());
        }

        void OnDisable() {

            //Disconnect from person updated event
            if (_initialized) {
                augmentaManager.augmentaObjectUpdate -= UpdateAugmentaObject;
            }
        }

        #endregion

        #region Scene Handling Functions

        /// <summary>
        /// Initialize the augmenta object
        /// </summary>
        void Initialize() {

            if (!augmentaManager)
                return;

            //Connect to Augmenta events
            augmentaManager.augmentaObjectUpdate += UpdateAugmentaObject;

            //Get an instance of the debug material
            augmentaObjectMaterialInstance = debugObject.GetComponent<Renderer>().material;

            //Apply a random color to the material
            Random.InitState(id);
            augmentaObjectMaterialInstance.SetColor("_Color", Color.HSVToRGB(Random.value, 0.85f, 0.75f));

            _initialized = true;
        }

        /// <summary>
        /// Response to augmenta object updated event
        /// </summary>
        /// <param name="augmentaObject"></param>
        public void UpdateAugmentaObject(AugmentaObject augmentaObject, AugmentaDataType augmentaDataType) {

            if (augmentaObject.id != id)
                return;

            //Update debug object size
            debugObject.transform.position = GetAugmentaObjectWorldPosition(true);
            debugObject.transform.localRotation = Quaternion.Euler(0.0f, -boundingRectRotation, 0.0f);
            debugObject.transform.localScale = GetAugmentaObjectWorldScale();

            //Update debug velocity
            debugVelocityPivot.transform.position = debugObject.transform.position;
            debugVelocity.transform.localPosition = new Vector3(0, highest.z * augmentaManager.scaling * 0.5f, velocity.magnitude * 0.5f);
            debugVelocityPivot.transform.localRotation = Quaternion.Euler(0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 90, 0);
            debugVelocity.transform.localScale = new Vector3(debugVelocity.transform.localScale.x, debugVelocity.transform.localScale.y, velocity.magnitude);

            //Update debug orientation
            debugOrientationPivot.transform.position = debugObject.transform.position;
            debugOrientation.transform.localPosition = new Vector3(0, highest.z * augmentaManager.scaling * 0.5f, 0.25f);
            debugOrientationPivot.transform.localRotation = Quaternion.Euler(0, orientation, 0);

        }

        /// <summary>
        /// Return the Augmenta object world position from the Augmenta scene position, offsetted by half the object height or not.
        /// </summary>
        /// <returns></returns>
        Vector3 GetAugmentaObjectWorldPosition(bool offset) {

            return augmentaManager.augmentaScene.transform.TransformPoint((centroid.x - 0.5f) * augmentaManager.augmentaScene.width * augmentaManager.scaling,
                                                                          offset ? highest.z * 0.5f * augmentaManager.scaling : 0,
                                                                          -(centroid.y - 0.5f) * augmentaManager.augmentaScene.height * augmentaManager.scaling);
        }

        /// <summary>
        /// Return the Augmenta object scale
        /// </summary>
        /// <returns></returns>
        Vector3 GetAugmentaObjectWorldScale() {

            return new Vector3(boundingRect.width * augmentaManager.augmentaScene.width * augmentaManager.scaling,
                               highest.z * augmentaManager.scaling,
                               boundingRect.height * augmentaManager.augmentaScene.height * augmentaManager.scaling);
        }

        #endregion

        #region Gizmos Functions

        public void DrawGizmoCube(Vector3 position, Quaternion rotation, Vector3 scale) {
            Matrix4x4 cubeTransform = Matrix4x4.TRS(position, rotation, scale);
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

            Gizmos.matrix *= cubeTransform;

            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

            Gizmos.matrix = oldGizmosMatrix;
        }

        #endregion

        #region Debug Functions

        /// <summary>
        /// Activate/desactivate debug object
        /// </summary>
        /// <param name="show"></param>
        public void ShowDebug(bool show) {

            debugObject.SetActive(show);
        }

        #endregion
    }
}