using SolarSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Solar_System
{
	public class StarRenderer : MonoBehaviour
	{
		private static readonly int Data = Shader.PropertyToID("StarData");
		private static readonly int Size = Shader.PropertyToID("size");
		private static readonly int BrightnessMultiplier = Shader.PropertyToID("brightnessMultiplier");
		private static readonly int RotationMatrix = Shader.PropertyToID("rotationMatrix");
		public SolarSystemManager solarSystemManager;
		public Shader starInstanceShader;
		public Light sun;
		//public Vector3 testParams;
		public float size;
		private Material _starMaterial;


		private Mesh _quadMesh;
		private ComputeBuffer _argsBuffer;
		private ComputeBuffer _starDataBuffer;
		private Camera _cam;

		public float brightnessMultiplier;
		public float appearTimeMin;
		public float appearTimeMax;

		public StarData starData;



		public void SetUpStarRenderingCommand(CommandBuffer cmd)
		{
			if (!Application.isPlaying) return;

			_cam = Camera.main;

			//stars = loader.LoadStars();
			CreateQuadMesh();
			EditorOnlyInit();

			_starMaterial = new Material(starInstanceShader);

			ComputeHelper.Release(_argsBuffer, _starDataBuffer);
			_argsBuffer = ComputeHelper.CreateArgsBuffer(_quadMesh, starData.NumStars);

			_starDataBuffer = ComputeHelper.CreateStructuredBuffer(starData.Stars);


			SetBuffer();

			cmd.DrawMeshInstancedIndirect(_quadMesh, 0, _starMaterial, 0, _argsBuffer, 0);
		}

		private void SetBuffer()
		{
			_starMaterial.SetBuffer(Data, _starDataBuffer);
		}


		public void UpdateFixedStars(EarthOrbit earth, bool geocentric)
		{
			if (!Application.isPlaying) return;
			_starMaterial.SetFloat(Size, size);
			_starMaterial.SetFloat(BrightnessMultiplier, brightnessMultiplier);
			var rotMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
			// Earth remains stationary and without rotation, so rotate the stars instead
			if (geocentric)
			{
				rotMatrix = Matrix4x4.Rotate(Quaternion.Inverse(earth.earthRot));
			}

			_starMaterial.SetMatrix(RotationMatrix, rotMatrix);


			//bounds.center = cam.transform.position;
			//Graphics.DrawMeshInstancedIndirect(quadMesh, 0, starMaterial, bounds, argsBuffer, castShadows: ShadowCastingMode.Off, receiveShadows: false);
			//Graphics.DrawMeshInstanced(quadMesh, 0, starInstanceShader,)//
		}


		private void CreateQuadMesh()
		{
			_quadMesh = new Mesh();

			Vector3[] vertices = {
			new(-1,-1), // bottom left
			new(1,-1), // bottom right
			new(1,1), // top left
			new(-1, 1) // top right
		};

			int[] triangles = { 0, 2, 1, 0, 3, 2 };

			_quadMesh.SetVertices(vertices);
			_quadMesh.SetTriangles(triangles, 0, true);
		}

		private void OnDestroy()
		{
			ComputeHelper.Release(_argsBuffer, _starDataBuffer);
		}

		private void EditorOnlyInit()
		{
#if UNITY_EDITOR
			EditorShaderHelper.onRebindRequired += SetBuffer;
#endif
		}
	}
}