using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision){
        GameObject collidedObject = collision.transform.parent.gameObject;
        if (collidedObject.gameObject.tag != "Wall"){
            Destroy(gameObject);
            return;
        }else{
            ToolHandler.OrbCollision(collidedObject.gameObject);
            Destroy(gameObject);
        }
        
        
    }
}
