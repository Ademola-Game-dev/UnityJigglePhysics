using System;
using System.Collections;
using System.Collections.Generic;
using GatorDragonGames.JigglePhysics;
using UnityEngine;
using Object = UnityEngine.Object;

public class StressTester : MonoBehaviour {
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private JiggleRigData data;

    private class JiggleDataHolder : MonoBehaviour, IJiggleParameterProvider {
        private JiggleRigData data;
        private JiggleTreeSegment segment;

        public void SetJiggleRigData(JiggleRigData data) {
            this.data = data;
            if (segment == null) {
                segment = new JiggleTreeSegment(this);
                segment.SetDirty();
                JigglePhysics.AddJiggleTreeSegment(segment);
            }
        }
        
        public JiggleRigData GetJiggleRigData() {
            return data;
        }

        private void OnDestroy() {
            JigglePhysics.RemoveJiggleTreeSegment(segment);
        }

        public bool HasAnimatedParameters => false;
    }

    private List<GameObject> startSnakes;
    private List<GameObject> snakes;

    private GameObject CreateChain(Vector3 position, int count) {
        var root = Object.Instantiate(cubePrefab, position, Quaternion.identity);
        var current = root;
        for (int i = 0; i < count; i++) {
            var obj = Object.Instantiate(cubePrefab, current.transform);
            obj.transform.SetLocalPositionAndRotation(Vector3.up * 0.5f, Quaternion.identity);
            current = obj;
        }

        JiggleRigData dataClone = data;
        dataClone.rootBone = root.transform;
        dataClone.BuildNormalizedDistanceFromRootList();
        root.AddComponent<Mover>();
        
        var holder = root.AddComponent<JiggleDataHolder>();
        holder.SetJiggleRigData(dataClone);
        return root;
    }

    IEnumerator Start() {
        snakes = new ();
        startSnakes = new ();
        int count = 0;
        for (int i = 0; i < 100; i++) {
            startSnakes.Add(CreateChain(Vector3.back + Vector3.right * count++, UnityEngine.Random.Range(1, 20)));
        }

        StartCoroutine(RemoveChunks());

        count = 0;
        while (isActiveAndEnabled) {
            for (int i = 0; i < UnityEngine.Random.Range(1, 8); i++) {
                yield return new WaitForSeconds(0.05f);
                snakes.Add(CreateChain(Vector3.forward + Vector3.right * count++, UnityEngine.Random.Range(1, 20)));
            }
        }
    }

    private IEnumerator RemoveChunks() {
        while (startSnakes.Count > 0) {
            for (int i = 0; i < UnityEngine.Random.Range(1, 6) && startSnakes.Count > 0; i++) {
                Destroy(startSnakes[0]);
                startSnakes.RemoveAt(0);
            }
            yield return new WaitForSeconds(0.25f);
        }
        startSnakes.Clear();
    }

    public JiggleRigData GetJiggleRigData() {
        return data;
    }
    public bool HasAnimatedParameters => false;

    private void OnValidate() {
        data.OnValidate();
    }
}
