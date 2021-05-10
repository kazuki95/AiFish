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

        RaycastHit hit = new RaycastHit();
        Vector3 direction = myManager.transform.position - transform.position;

        if (!b.Contains(transform.position))
        {
//usando raycast e reflect para que o cardume evite de passar por dentro do pilar
            turning = true;
            direction = myManager.transform.position - transform.position;
        }
        else if (Physics.Raycast(transform.position, this.transform.forward * 50, out hit))
        {
            turning = true;
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
        }

        else
            turning = false;
        // rotação do turning
        if (turning)        
        {       
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
        //array criado
        GameObject[] gos;
        gos = myManager.allFish;
//dados da movimentação dos peixes
        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;
//fazendo os peixes entrarem no cardume
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

                // rotação do grupo de peixes
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
