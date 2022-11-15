using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoidsManager : MonoBehaviour
{
    public GameObject boidPrefab;
    public int boidCount = 10;
    public float spawnrange = 5f;
    public Transform goal;

    public ComputeShader boidShader;

    private List<GameObject> visualList = new List<GameObject>();
    private Boid[] boids;
    private Vector3[] velocities;
    
    public float minDist;
    public float centerForce;
    public float flockAmount;
    public float goalAmount;

    struct Boid
    {
        public Vector3 position;
        public Vector3 velocity;
    }

    private float minDistSqr;
    void Start()
    {
        boids = new Boid[boidCount];
        for (int i = 0; i < boidCount; i++)
        {
            visualList.Add(Instantiate(boidPrefab,Vector3.zero,Quaternion.identity));
            boids[i] = new Boid();
            boids[i].position = new Vector3(Random.Range(-spawnrange,spawnrange),Random.Range(-spawnrange,spawnrange),Random.Range(-spawnrange,spawnrange));
            //boids[i].velocity = new Vector3(Random.Range(-2,2),Random.Range(-2,2),Random.Range(-2,2));
        }
        boidShader.SetFloat("N",boidCount - 1);
        boidShader.SetFloat("NMinusOne", boidCount - 2);
        minDistSqr = minDist * minDist;
    }
    
    void Update()
    {
        ComputeBuffer buf = new ComputeBuffer(boidCount, sizeof(float) * 3 * 2);
        buf.SetData(boids);
        boidShader.SetBuffer(0,"boids", buf);
        boidShader.SetFloat("deltaTime", Time.deltaTime);
        boidShader.SetFloat("minDistSqr", minDistSqr);
        boidShader.SetFloat("centerForce", centerForce);
        boidShader.SetFloat("flockAmount", flockAmount);
        boidShader.SetVector("goalPos",goal.position);
        boidShader.SetFloat("goalAmount",goalAmount);
        boidShader.Dispatch(0,boidCount,1,1);
        buf.GetData(boids);
        for (int i = 0; i < boidCount; i++)
        {
            visualList[i].transform.position = boids[i].position;
            visualList[i].transform.rotation = Quaternion.Slerp(visualList[i].transform.rotation,Quaternion.FromToRotation(-Vector3.forward, boids[i].velocity), 10f * Time.deltaTime);
        }

        boids[0].position = transform.position;
        buf.Dispose();
    }
}
