using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Utility 
{
    public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance, int areaMask)  // 인수 👉 중심 위치, 반경 거리, 검색할 Area (내부적으로 int)
    {
        Vector3 randomPos = Random.insideUnitSphere * distance + center;  // center를 중점으로 하여 반지름(반경) distance 내에 랜덤한 위치 리턴. *Random.insideUnitSphere*은 반지름 1 짜리의 구 내에서 랜덤한 위치를 리턴해주는 프로퍼티다.
       
        NavMeshHit hit;  // NavMesh 샘플링의 결과를 담을 컨테이너. Raycast hit 과 비슷

        NavMesh.SamplePosition(randomPos, out hit, distance, areaMask);  // areaMask에 해당하는 NavMesh 중에서 randomPos로부터 distance 반경 내에서 randomPos에 *가장 가까운* 위치를 하나 찾아서 그 결과를 hit에 담음. 

        return hit.position;  // 샘플링 결과 위치인 hit.position 리턴
    }
}
