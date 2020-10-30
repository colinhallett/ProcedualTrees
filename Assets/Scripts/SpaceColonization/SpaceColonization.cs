using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CKZH.ProcedualTree.SpaceColonization
{
    [ExecuteInEditMode]
    public class SpaceColonization : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private int amountOfPoints;
        [SerializeField] private int iterations = 20;
        [SerializeField] private float branchLength = 0.2f;
        [SerializeField] private float attractionRadius = 0.5f;

        [SerializeField] private int radialSubdivisions = 5;
        [SerializeField] private float extremityRadius = 0.1f;
        [Range(0, 1f)]
        [SerializeField] private float invertedGrowthFactor = 0.5f;
        private float normalisedGrowth => Easing.Exponential.InOut(invertedGrowthFactor);
        [Range(0,1f)]
        [SerializeField] private float baseThickness = 1f;
        private float normalisedBase => baseThickness * 0.2f - 0.1f + 1f;

        [SerializeField] private float iterationTime = 0.5f;
        [SerializeField] private bool gizmos;
        [SerializeField] private bool showAttractionArea;

        [SerializeField] private Transform treeStart;

        private List<AttractionPoint> attractionPoints;
        private Vector3 startingPos => treeStart.position;
        private float killRange => attractionRadius * 0.7f;

        private List<TreeNode> nodes;
        private List<TreeNode> nodesWithAttraction;

        public void Generate()
        {
            GenerateSphere();
            nodes = new List<TreeNode>();
            StopAllCoroutines();
            
            if (Application.isPlaying)
            {
                StartCoroutine(GenerateBranchesCR(iterations));
            }
            
        }

        private IEnumerator GenerateBranchesCR(int iterationsRemaining)
        {
            nodesWithAttraction = new List<TreeNode>();

            TreeNode previous = null;
            
            for (int i = 0; i < iterationsRemaining; i++)
            {
                if (nodesWithAttraction.Count <= 0)
                {
                    var newNode = GenerateNode(previous);
                    nodes.Add(newNode);
                    if (previous != null)
                        previous.hasConnection = true;
                    previous = newNode;
                }
                else
                {
                    for (int a = 0; a < nodesWithAttraction.Count; a++)
                    {
                        //if (nodesWithAttraction[a].hasConnection) continue;
                        var newBranch = GenerateNode(nodesWithAttraction[a]);
                        nodesWithAttraction[a].hasConnection = true;
                        nodes.Add(newBranch);
                        RemoveAttractionPointsInKillZone(nodesWithAttraction[a]);
                    }
                }

                
                ClearAllNodesAttractors();
                GenerateMesh();
                if (attractionPoints.Count == 0) break;
                UpdateAttractionNodes();
                yield return new WaitForSeconds(iterationTime);
            }
          //  GenerateMesh();
        }

        private void ClearAllNodesAttractors()
        {
            for (int i = 0; i < nodesWithAttraction.Count; i++)
            {
                nodesWithAttraction[i].attractors.Clear();
            }
            nodesWithAttraction.Clear();
        }

        private void UpdateAttractionNodes()
        {
            List<TreeNode> toAdd = new List<TreeNode>();
            for (int i = 0; i < attractionPoints.Count; i++)
            {
                var closest = GetClosestNode(attractionPoints[i]);
                if (closest != null)
                {
                    closest.attractors.Add(attractionPoints[i]);
                    attractionPoints[i].inRange = true;
                    if (!toAdd.Contains(closest))
                        toAdd.Add(closest);
                }
            }

            UpdateNodesWithAttractionList(toAdd);
        }

        private void UpdateNodesWithAttractionList(List<TreeNode> toAdd)
        {
            for (int i = 0; i < toAdd.Count; i++)
            {
                nodesWithAttraction.Add(toAdd[i]);
            }
        }

        private TreeNode GetClosestNode(AttractionPoint point)
        {
            TreeNode node = null;
            var currentDistanct = attractionRadius;
            for (int n = 0; n < nodes.Count; n++)
            {
                //if (nodes[n].hasConnection) continue;
                var test = Vector3.Distance(point.position, nodes[n].end);
                if (test < currentDistanct)
                {
                    node = nodes[n];
                    currentDistanct = test;
                }
            }

            return node;
        }

        private void RemoveAttractionPointsInKillZone(TreeNode node)
        {
            List<AttractionPoint> toRemove = new List<AttractionPoint>();
            for (int a = 0; a < node.attractors.Count; a++)
            {
                if (Vector3.Distance(node.end, node.attractors[a].position) < killRange)
                {
                    toRemove.Add(node.attractors[a]);
                }
            }

            for (int r = 0; r < toRemove.Count; r++)
            {
                attractionPoints.Remove(toRemove[r]);
            }
        }
    

        private Vector3 GetAverageDirectionFromAttractors(TreeNode node)
        {
            Vector3 direction = Vector3.zero;
            for (int a = 0; a < node.attractors.Count; a++)
            {
                direction += node.attractors[a].position - node.end;
            }
            direction.Normalize();
            if (direction == Vector3.zero)
                direction = (Random.insideUnitSphere * 0.1f + node.direction).normalized;
            return direction;
        }

        private TreeNode GenerateNode(TreeNode previous)
        {
            TreeNode newNode;
            Vector3 direction;

            if (previous == null)
            {
                direction = Vector3.up;
                newNode = new TreeNode(startingPos, startingPos + direction * branchLength, direction, null);
                return newNode;
            }

            if (previous.attractors.Count <= 0)
            {
                direction = (Random.insideUnitSphere * 0.1f + previous.direction).normalized;
            }
            else
            {
                direction = GetAverageDirectionFromAttractors(previous);
            }
            newNode = new TreeNode(previous.end, previous.end + direction * branchLength, direction, previous);
            
            return newNode;
        }

        private void GenerateSphere()
        {
            attractionPoints = new List<AttractionPoint>(amountOfPoints);

            for (int i = 0; i < amountOfPoints; i++)
            {
                //https://karthikkaranth.me/blog/generating-random-points-in-a-sphere/
                var u = Random.Range(0, 1f);
                var v = Random.Range(0, 1f);
                var theta = u * 2.0f * Mathf.PI;
                var phi = Mathf.Acos(2.0f * v - 1.0f);
                var r= Mathf.Pow(Random.Range(0, 1f), 1.0f / 3.0f);
                var sinTheta = Mathf.Sin(theta);
                var cosTheta = Mathf.Cos(theta);
                var sinPhi = Mathf.Sin(phi);
                var cosPhi = Mathf.Cos(phi);
                var x = r * sinPhi * cosTheta;
                var y = r * sinPhi * sinTheta;
                var z = r * cosPhi;

                var pos = new Vector3(x, y, z) * radius + transform.position; 

                AttractionPoint node = new AttractionPoint(pos);
                attractionPoints.Add(node);
            }
        }

        private void GenerateMesh()
        {
            Vector3[] vertices = new Vector3[(nodes.Count + 1) * radialSubdivisions];
            int[] triangles = new int[(nodes.Count)* radialSubdivisions * 6];

            for (int n = nodes.Count - 1; n >= 0; n--)
            {
                TreeNode node = nodes[n];

                float branchSize = 0f;

                if (node.children.Count == 0)
                {
                    branchSize = extremityRadius * normalisedBase;
                }
                else
                {
                    branchSize = Mathf.Pow(node.children[0].radius, 1 - normalisedGrowth) * normalisedBase;
                }

                node.radius = branchSize;
            }

            for (int n = nodes.Count - 1; n >= 0; n--)
            {
                TreeNode node = nodes[n];
                int vID = radialSubdivisions * (n + 1);
                node.verticesId = vID;

                Quaternion quat = Quaternion.FromToRotation(Vector3.up, node.direction);

                for (int r = 0; r < radialSubdivisions; r++)
                {
                    float alpha = ((float)r / radialSubdivisions) * Mathf.PI * 2f;

                    Vector3 pos = new Vector3(Mathf.Cos(alpha) * node.radius, 0, Mathf.Sin(alpha) * node.radius);
                    pos = quat * pos;
                    pos += node.end;
                    vertices[vID + r] = pos - transform.position;

                    if (node.parent == null)
                    {
                        vertices[r] = node.start + new Vector3(Mathf.Cos(alpha) * node.radius, 0, Mathf.Sin(alpha) * node.radius) - transform.position;
                        
                    }
                }
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                var currentNode = nodes[i];
                var parentNode = currentNode.parent;

                var currentIndex = currentNode.verticesId;
                var parentIndex = parentNode == null ? 0 : parentNode.verticesId;

                

                for (int r = 0; r < radialSubdivisions; r++)
                {
                    var startIndex = i * 6 * radialSubdivisions + r * 6;


                    if (r != radialSubdivisions - 1)
                    {
                        triangles[startIndex + 0] = parentIndex + r + 0;
                        triangles[startIndex + 1] = currentIndex + r + 0;
                        triangles[startIndex + 2] = parentIndex + r + 1;
                        
                        triangles[startIndex + 3] = parentIndex + r + 1;
                        triangles[startIndex + 4] = currentIndex + r + 0;
                        triangles[startIndex + 5] = currentIndex + r + 1;
                    }
                    else
                    {
                        triangles[startIndex + 0] = parentIndex + r + 0;
                        triangles[startIndex + 1] = currentIndex + r + 0;
                        triangles[startIndex + 2] = parentIndex;

                        triangles[startIndex + 3] = parentIndex;
                        triangles[startIndex + 4] = currentIndex + r + 0;
                        triangles[startIndex + 5] = currentIndex;
                    }
                }
            }

            MeshFilter filter = GetComponent<MeshFilter>();
            Mesh mesh = filter.mesh;
            mesh.Clear();

            mesh.vertices = vertices;
            //mesh.normals = normales;
            //mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.Optimize();

        }

        private void OnDrawGizmos()
        {
            if (!gizmos) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
            if (attractionPoints == null) return;

            for (int i = 0; i < attractionPoints.Count; i++)
            {
                var n = attractionPoints[i];
                Gizmos.color = n.inRange ? Color.red : Color.yellow;
                Gizmos.DrawWireSphere(n.position, 0.1f);
            }

            if (nodes == null) return;

            for (int i = 0; i < nodes.Count; i++)
            {
                
                
                if (nodes[i].attractors.Count > 0)
                {
                    if (showAttractionArea)
                    {
                        Gizmos.color = new Color(0, 0, 0, 0.1f);
                        Gizmos.DrawSphere(nodes[i].end, attractionRadius);
                        Gizmos.color = new Color(0, 0, 0, 0.2f);
                        Gizmos.DrawSphere(nodes[i].end, killRange);
                    }
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(nodes[i].end, 0.2f);
                }
                   
                else
                {
                  //  Gizmos.color = Color.magenta;
                  //  Gizmos.DrawSphere(nodes[i].end, 0.03f);
                }
                
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(nodes[i].start, nodes[i].end);
            }
        }
    }
    public class AttractionPoint
    {
        public Vector3 position;
        public bool inRange;

        public AttractionPoint(Vector3 position)
        {
            this.position = position;
        }
    }
}