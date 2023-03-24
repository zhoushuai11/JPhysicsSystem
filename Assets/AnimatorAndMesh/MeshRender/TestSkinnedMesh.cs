using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkinnedMesh : MonoBehaviour {
    // Start is called before the first frame update
    
    private struct DoublePoseStaticData {
        public long pid;
        public long toPid;
        public int relationType;
        public bool isInitiate;
        public bool isAgree;
        public int fallReason; // 0:主动拒绝 1:超时 2：不符合条件 3:超出距离 -1:成功
        public bool isSuccessShow;
    }

    void Start() {
        var list = new List<DoublePoseStaticData>();
        list.Add(new DoublePoseStaticData {
            pid = 1,
            toPid = 2,
            relationType = 3
        });
        list.Add(new DoublePoseStaticData {
            pid = 1,
            toPid = 2,
            relationType = 3
        });
        DebugEx.LogError(list.FindLastIndex(data => data is { pid: 1, toPid: 2 }));
    }

   
    private GameObject gameObject;

    private void CreateSkinnedMesh() {
        gameObject = new GameObject("SkinnedMeshObj");
        gameObject.transform.eulerAngles = new Vector3(0, -180, 0);
        var skinnedMeshRender = gameObject.AddComponent<SkinnedMeshRenderer>();
        var anim = gameObject.AddComponent<Animation>();

        //新建个网格组件，并编入4个顶点形成一个矩形形状的网格
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] {
            new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(-1, 5, 0), new Vector3(1, 5, 0)
        };
        mesh.uv = new Vector2[] {
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
        };
        mesh.triangles = new int[] {
            0, 1, 2, 1, 3, 2
        };
        mesh.RecalculateNormals();

        //新建个漫反射的材质球
        skinnedMeshRender.material = new Material(Shader.Find("Diffuse"));

        //为每个顶点定制相应的骨骼权重
        BoneWeight[] weights = new BoneWeight[4];
        weights[0].boneIndex0 = 0;
        weights[0].weight0 = 1;
        weights[1].boneIndex0 = 0;
        weights[1].weight0 = 1;
        weights[2].boneIndex0 = 1;
        weights[2].weight0 = 1;
        weights[3].boneIndex0 = 1;
        weights[3].weight0 = 1;

        //把骨骼权重赋值给网格组件
        mesh.boneWeights = weights;

        //创建新的骨骼点，设置骨骼点的位置，父节点，和位移旋转矩阵
        Transform[] bones = new Transform[2];
        Matrix4x4[] bindPoses = new Matrix4x4[2];

        bones[0] = new GameObject("Lower").transform;
        bones[0].parent = gameObject.transform;
        bones[0].localRotation = Quaternion.identity;
        bones[0].localPosition = Vector3.zero;
        bindPoses[0] = bones[0].worldToLocalMatrix * gameObject.transform.localToWorldMatrix;

        bones[1] = new GameObject("Upper").transform;
        bones[1].parent = gameObject.transform;
        bones[1].localRotation = Quaternion.identity;
        bones[1].localPosition = new Vector3(0, 5, 0);
        bindPoses[1] = bones[1].worldToLocalMatrix * gameObject.transform.localToWorldMatrix;
        mesh.bindposes = bindPoses;

        //把骨骼点和网格赋值给蒙皮组件
        skinnedMeshRender.rootBone = bones[0];
        skinnedMeshRender.bones = bones;
        skinnedMeshRender.sharedMesh = mesh;

        //定制几个关键帧
        AnimationCurve curve = new AnimationCurve();
        curve.keys = new Keyframe[] {
            new Keyframe(0, 0, 0, 0), new Keyframe(1, 3, 0, 0), new Keyframe(2, 0.0F, 0, 0)
        };

        //创建帧动画
        AnimationClip clip = new AnimationClip();
        clip.SetCurve("Lower", typeof(Transform), "m_LocalPosition.z", curve);
        //把帧动画赋值给动画组件，并播放动画
        anim.AddClip(clip, "test");
        clip.name = "test";
        anim.clip = clip;

        anim.Play();

        // transform.TransformPoint()
    }
}