using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hold the scene values from the Augmenta protocol and update the scene debug view.
/// </summary>
namespace Augmenta
{
	public class AugmentaScene : MonoBehaviour
	{

		[Header("Scene Settings")]
		public AugmentaManager augmentaManager;
		public bool showDebug = false;
		public GameObject debugObject;

		[Header("Augmenta Scene Values")]
		public float width;
		public float height;
		public int augmentaObjectCount; //Object count from the scene updated message  /!\ Because of personTimeOut, it can be different from the instantiated person count /!\

		private Material _debugMaterial;

		private bool _isHDRP = false;

		private bool _initialized = false;

		#region MonoBehaviour Functions

		private void OnEnable() {

			_initialized = false;
		}

		private void Update() {

			//Initialization
			if (!_initialized)
				Initialize();

			//Update debug state if incoherent
			if (showDebug != debugObject.activeSelf)
				ShowDebug(showDebug);
		}

		private void OnDisable() {

			//Disconnect from Augmenta SceneUpdated event
			if(_initialized)
				augmentaManager.sceneUpdated -= UpdateScene;
		}

		void OnDrawGizmos() {

			Gizmos.color = Color.blue;
			DrawGizmoCube(transform.position, transform.rotation, new Vector3(width * augmentaManager.scaling, 0, height * augmentaManager.scaling));
		}

		#endregion

		#region Scene Handling Functions

		/// <summary>
		/// Initialize the scene
		/// </summary>
		void Initialize() {

			if (!augmentaManager)
				return;

			//Connect to Augmenta SceneUpdated event
			augmentaManager.sceneUpdated += UpdateScene;

			//Get the debug material
			_debugMaterial = debugObject.GetComponent<Renderer>().material;

			//Check if HDRP is used from scene debug object shader type
			_isHDRP = _debugMaterial.shader.name.Contains("HDRP");

			_initialized = true;
		}

		/// <summary>
		/// Update the scene object.
		/// </summary>
		public void UpdateScene() {

			//Initialization
			if (!_initialized)
				Initialize();

			//Update debug object size
			debugObject.transform.localScale = new Vector3(width * augmentaManager.scaling, height * augmentaManager.scaling, 0);

			//Update debug material tiling
			if (_isHDRP) {
				_debugMaterial.SetTextureScale("_UnlitColorMap", debugObject.transform.localScale * 0.5f);
				_debugMaterial.SetTextureOffset("_UnlitColorMap", Vector2.down * debugObject.transform.localScale.y * 0.5f);
			} else {
				_debugMaterial.mainTextureScale = debugObject.transform.localScale * 0.5f;
				_debugMaterial.mainTextureOffset = Vector2.down * debugObject.transform.localScale.y * 0.5f;
			}
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
