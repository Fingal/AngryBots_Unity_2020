using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

public class CreateImage : MonoBehaviour {
    public int density = 1000;
    public int iterationNumber = 10;
    public float scale = 4;
    public bool useBurst = false;
    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct GenerateTextureJob: IJob {
        [WriteOnly]
        public NativeArray<float> colour;
        public int iterationNumber;
        public int density;
        public float scale;
        [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
        public void Execute() {
            for (int i = 0; i < density * density; i++) {
                int x = i % density;
                int y = (i / density);
                float xCoord = (x / (float)(density) - 0.5f) * scale;
                float yCoord = (y / (float)(density) - 0.5f) * scale;
                float2 coord; //= (xCoord, yCoord);
                coord.x = xCoord;
                coord.y = yCoord;
                float2 startCoord = coord;
                int result = 0;
                for (int _ = 0; _ < iterationNumber; _++) {
                    if (math.length(coord) > 1000) {
                        break;
                    }
                    coord.x = coord.x * coord.x - coord.y * coord.y + startCoord.x;
                    coord.y = coord.x * coord.y * 2 + startCoord.y;
                    result += 1;
                }
                colour[i] = 1 - result / (float)iterationNumber;
            }
        }

    }
    // Start is called before the first frame update
    void Update() {
        NativeArray<float> colour = new NativeArray<float>(density*density, Allocator.TempJob);
        if (useBurst) {
            var job = new GenerateTextureJob {
                colour = colour,
                iterationNumber = iterationNumber,
                density = density,
                scale = scale,
            };
            JobHandle handle = job.Schedule();
            handle.Complete();
        }

        else {
            for (int i = 0; i < density * density; i++) {
                int x = i % density;
                int y = (i / density);
                float xCoord = (x / (float)(density) - 0.5f) * scale;
                float yCoord = (y / (float)(density) - 0.5f) * scale;
                float2 coord; //= (xCoord, yCoord);
                coord.x = xCoord;
                coord.y = yCoord;
                float2 startCoord = coord;
                int result = 0;
                for (int _ = 0; _ < iterationNumber; _++) {
                    if (math.length(coord) > 1000) {
                        break;
                    }
                    coord.x = coord.x * coord.x - coord.y * coord.y + startCoord.x;
                    coord.y = coord.x * coord.y * 2 + startCoord.y;
                    result += 1;
                }
                colour[i] = 1 - result / (float)iterationNumber;
            }
        }

        var texture = new Texture2D(density, density, TextureFormat.RFloat,false);
        for (int i = 0; i < density; i++) {
            for (int j = 0; j < density; j++) {
                texture.SetPixel(i, j, new Color(colour[i + j * density], colour[i + j * density], colour[i + j * density]));
            }
        }
        texture.Apply();

        colour.Dispose();
        gameObject.GetComponent<Image>();
        gameObject.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, density, density), new Vector2(0.5f, 0.5f));
    }

}
