using UnityEngine;
using System.Collections;
//using UnityEditor;
public class Liner : MonoBehaviour
{
    public Mesh mesh;
    // Use this for initialization
    void Start()
    {

    }
    /// <summary>
    /// 如果你想绘制可被点选的gizmos，执行这个函数
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = Vector3.right * 2;  //世界坐标系的 轴向x
        Gizmos.DrawRay(transform.position, direction);
        Gizmos.color = Color.green;
        direction = Vector3.up * 2;   //世界坐标系的 轴向y
        Gizmos.DrawRay(transform.position, direction);
        Gizmos.color = Color.blue;
        direction = Vector3.forward * 2;     //世界坐标系的 轴向z
        Gizmos.DrawRay(transform.position, direction);



    }


    /// <summary>
    /// 如果你想在物体被选中时绘制gizmos，执行这个函数
    /// </summary>
    void OnDrawGizmosSelected()
    {


        //Gizmos.color = Color.white;
        //Gizmos.DrawSphere(transform.position, 1);



    }
    // Update is called once per frame
    void Update()
    {

    }
}