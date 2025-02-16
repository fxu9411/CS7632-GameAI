using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using GameAICourse;

namespace Tests
{
    public class NavmeshTest
    {
        // You can run the tests in this class in the Unity Editor if you open
        // the Test Runner Window and choose the EditMode tab


        // Annotate methods with [Test] or [TestCase(...)] to create tests like this one!
        [Test]
        public void TestName()
        {
            // Tests are performed through assertions. You can Google NUnit Assertion
            // documentation to learn about them. Several examples follow.
            Assert.That(CreateNavMesh.StudentAuthorName, Is.Not.Contains("George P. Burdell"),
                "You forgot to change to your name!");
        }



        // TODO Write some tests! See GridTest and PathNetworkTest for more examples
        // PathNetworkTest(s) you have previously worked on will be useful
        // to validate you are creating a good path graph from your navmesh!

        [TestCase(true)]
        [TestCase(false)]
        public void TestNavMesh(bool useExpandedGeomtery)
        {

            Vector2 origin = new Vector2(-5f, -5f);
            Vector2 size = new Vector2(10f, 10f);
            float agentRadius = 1f;

            if (!useExpandedGeomtery)
                agentRadius = 0f;


            List<Polygon> obstacles = new List<Polygon>();

            Polygon tri = new Polygon();

            Vector2[] triPts =
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 1f)

            };

            tri.SetPoints(triPts);

            Assert.That(CG.Ccw(triPts), Is.True, "SETUP FAILURE: polygon verts not listed CCW");

            obstacles.Add(tri);

            var offsetObst = Utils.GenerateExpandedGeometry(
                    origin, size.x, size.y,
                    agentRadius, obstacles
                    );

            List<Polygon> origTris;
            List<Polygon> navMeshPolys;
            AdjacentPolygons adjPolys;
            List<Vector2> pnodes;
            List<List<int>> pedges;

            CreateNavMesh.Create(origin, size.x, size.y,
                    offsetObst, agentRadius,
                    out origTris,
                    out navMeshPolys,
                    out adjPolys,
                    out pnodes, out pedges
                    );

            Debug.Log($"Num Tris: {origTris.Count}");
            Debug.Log($"Num NavMeshPolys: {navMeshPolys.Count}");
            Debug.Log($"Num AdjPolys.Keys: {adjPolys.Keys.Count}");
            Debug.Log($"Num pnodes: {pnodes.Count}");
            Debug.Log($"Num pedges: {pedges.Count}");

            // TODO do validation
        }

    }
}
