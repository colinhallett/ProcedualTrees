using UnityEngine;

namespace CKZH.ProcedualTree
{
    public class Leaf : Module
    {
		[SerializeField] float startWidth = 0.2f;
		[SerializeField] float endWidth = 0.01f;
		[SerializeField] Material mat;
		[SerializeField] private float scaleReduction = 0.99f;
		

		public override void Generate(Seed seed)
        {
			Vector3 startPos = seed.currentTransform.position;
			Quaternion startRot = seed.currentTransform.rotation;

			float scale = seed.currentTransform.currentScale;

			Vector3 direction = startRot * seed.transform.up;
			Vector3 endPos = startPos + (direction * startWidth * scale);
			Quaternion endRot = Quaternion.FromToRotation(startPos, endPos);

			DrawTriangle(startWidth * scale, endWidth * scale);
			
			var ren = gameObject.AddComponent<MeshRenderer>();
			ren.sharedMaterial = mat;

			transform.position = startPos;
			transform.rotation = startRot * Quaternion.Euler(Random.insideUnitSphere * 120f);

			seed.currentTransform.currentScale *= scaleReduction;

			transform.SetParent(seed.currentTransform.parent);
			seed.currentTransform.parent = transform;
			if (setStatic)
				gameObject.isStatic = true;
		}

		private void DrawTriangle(float width, float height)
        {
			MeshFilter filter = gameObject.AddComponent<MeshFilter>();
			Mesh mesh = filter.mesh;
			mesh.Clear();

			//We need two arrays one to hold the vertices and one to hold the triangles
			Vector3[] verteicesArray = new Vector3[6];
			int[] trianglesArray = new int[6];
			//lets add 3 vertices in the 3d space
			verteicesArray[0] = new Vector3(0, 0, 0);
			verteicesArray[1] = new Vector3(1 * width, 0, 0);
			verteicesArray[2] = new Vector3(1 * height / 2, 1 * height, 0);
			verteicesArray[3] = new Vector3(0, 0, 0);
			verteicesArray[4] = new Vector3(1 * height / 2, 1 * height, 0);
			verteicesArray[5] = new Vector3(1 * width, 0, 0);
			//define the order in which the vertices in the VerteicesArray shoudl be used to draw the triangle
			trianglesArray[0] = 0;
			trianglesArray[1] = 1;
			trianglesArray[2] = 2;
			trianglesArray[3] = 3;
			trianglesArray[4] = 4;
			trianglesArray[5] = 5;
			//add these two triangles to the mesh
			mesh.vertices = verteicesArray;
			mesh.triangles = trianglesArray;

			mesh.RecalculateBounds();
			mesh.Optimize();
		}

		private void DrawPlane(float width, float length)
        {
			// You can change that line to provide another MeshFilter
			MeshFilter filter = gameObject.AddComponent<MeshFilter>();
			Mesh mesh = filter.mesh;
			mesh.Clear();

			int resX = 2; // 2 minimum
			int resZ = 2;

			#region Vertices		
			Vector3[] vertices = new Vector3[resX * resZ];
			for (int z = 0; z < resZ; z++)
			{
				// [ -length / 2, length / 2 ]
				float zPos = ((float)z / (resZ - 1) - .5f) * length;
				for (int x = 0; x < resX; x++)
				{
					// [ -width / 2, width / 2 ]
					float xPos = ((float)x / (resX - 1) - .5f) * width;
					vertices[x + z * resX] = new Vector3(xPos, 0f, zPos);
				}
			}
			#endregion

			#region Normales
			Vector3[] normales = new Vector3[vertices.Length];
			for (int n = 0; n < normales.Length; n++)
				normales[n] = Vector3.up;
			#endregion

			#region UVs		
			Vector2[] uvs = new Vector2[vertices.Length];
			for (int v = 0; v < resZ; v++)
			{
				for (int u = 0; u < resX; u++)
				{
					uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
				}
			}
			#endregion

			#region Triangles
			int nbFaces = (resX - 1) * (resZ - 1);
			int[] triangles = new int[nbFaces * 6];
			int t = 0;
			for (int face = 0; face < nbFaces; face++)
			{
				// Retrieve lower left corner from face ind
				int i = face % (resX - 1) + (face / (resZ - 1) * resX);

				triangles[t++] = i + resX;
				triangles[t++] = i + 1;
				triangles[t++] = i;

				triangles[t++] = i + resX;
				triangles[t++] = i + resX + 1;
				triangles[t++] = i + 1;
			}
			#endregion

			mesh.vertices = vertices;
			mesh.normals = normales;
			mesh.uv = uvs;
			mesh.triangles = triangles;

			mesh.RecalculateBounds();
			mesh.Optimize();
		}
    }
}
