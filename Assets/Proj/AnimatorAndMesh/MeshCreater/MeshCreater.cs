using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreater : MonoBehaviour {
    public Material setMat;
    
    private void Start() {
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        var meshRender = gameObject.AddComponent<MeshRenderer>();
        
        var height = 1;
        var width = 1;
        var size = 10;

        var offsetCol = 1;
        var offsetValue = 10;
        
        var vertexCount = height * width;
        var mesh = new Mesh();
        
        var vertices = new Vector3[4 * vertexCount];
        var uvs = new Vector2[4 * vertexCount];
        var normals = new Vector3[4 * vertexCount];
        var tangents = new Vector4[4 * vertexCount];
        var triangles = new int[6 * vertexCount];

        var uvRowRatio = 1.0f / width;
        var uvColRatio = 1.0f / height;
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                var index = i * height + j;
                var offset = 0.0f;
                if (j == offsetCol) {
                    offset = offsetValue;
                }
        
                vertices[index * 4 + 0] = new Vector3(size * i, size * j + offset);
                vertices[index * 4 + 1] = new Vector3(size * i, size * (j + 1) + offset);
                vertices[index * 4 + 2] = new Vector3(size * (i + 1), size * (j + 1) + offset);
                vertices[index * 4 + 3] = new Vector3(size * (i + 1), size * j + offset);

                var uvX = uvRowRatio * i;
                var uvNextX = uvRowRatio * (i + 1);
                var uvY = uvColRatio * j;
                var uvNextY = uvColRatio * (j + 1);
                uvs[index * 4 + 0] = new Vector2(uvX, uvY);
                uvs[index * 4 + 1] = new Vector2(uvX, uvNextY);
                uvs[index * 4 + 2] = new Vector2(uvNextX, uvNextY);
                uvs[index * 4 + 3] = new Vector2(uvNextX, uvY);
        
                normals[index * 4 + 0] = Vector3.back;
                normals[index * 4 + 1] = Vector3.back;
                normals[index * 4 + 2] = Vector3.back;
                normals[index * 4 + 3] = Vector3.back;
                
                tangents[index * 4 + 0] = new Vector4(1, 0, 0, -1);
                tangents[index * 4 + 1] = new Vector4(1, 0, 0, -1);
                tangents[index * 4 + 2] = new Vector4(1, 0, 0, -1);
                tangents[index * 4 + 3] = new Vector4(1, 0, 0, -1);
        
                triangles[index * 6 + 0] = index * 4 + 0;
                triangles[index * 6 + 1] = index * 4 + 1;
                triangles[index * 6 + 2] = index * 4 + 2;
                
                triangles[index * 6 + 3] = index * 4 + 0;
                triangles[index * 6 + 4] = index * 4 + 2;
                triangles[index * 6 + 5] = index * 4 + 3;
            }
        }
        
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.tangents = tangents;
        
        meshFilter.sharedMesh = mesh;
        meshRender.materials = new Material[] {
            setMat,
        };
    }
}