using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockManager myManager;
    public float speed;
    bool turning = false;
    void Start() {
        speed = Random.Range (myManager.minSpeed, 
            myManager.maxSpeed);
    }
    void Update() 
    {
        // limite maximo que o peixe consegue nadar
        Bounds b = new Bounds(myManager.transform.position, myManager.swinLimits * 2);

        if (!b.Contains(transform.position))
        {
            turning = true;
        }
        else
            turning = false;
        // rotação do turning
        if (turning) 
        {
            Vector3 direction = myManager.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                Quaternion.LookRotation(direction), 
                myManager.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 100) > 10)
                speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
            if(Random.Range(0, 100)< 20)
                ApplyRules();
        }      
        transform.Translate(0, 0, Time.deltaTime * speed); 
    }

    void ApplyRules()
    {
        //array
        GameObject[] gos;
        gos = myManager.allFish;

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            //distancia dos peixes e seus vizinhos
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (nDistance <= myManager.neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if (go != this.gameObject)
                    {
                        nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                        
                        if (nDistance <= myManager.neighbourDistance)
                        {
                            vcentre += go.transform.position; groupSize++;

                            if (nDistance < 1.0f)
                            {
                                vavoid = vavoid + (this.transform.position - go.transform.position);
                            }
                            //encontrar um novo posicionamento
                            Flock anotherFlock = go.GetComponent<Flock>(); 
                            gSpeed = gSpeed + anotherFlock.speed;
                        }
                    }
                }

                // contagem do grupo de peixes
                if (groupSize > 0) 
                    {
                    vcentre = vcentre / groupSize + (myManager.goalPos - this.transform.position);
                    speed = gSpeed / groupSize;

                    Vector3 direction = (vcentre + vavoid) - transform.position;

                    // rotação do peixe mais suave
                    if(direction != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, 
                            Quaternion.LookRotation(direction), 
                            myManager.rotationSpeed * Time.deltaTime);
                    }
                }
            }
        }
    }
}
