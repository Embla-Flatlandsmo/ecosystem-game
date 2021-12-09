#if !defined(WATER_UTILS)
#define WATER_UTILS
#include "UnityPBSLighting.cginc"

float Foam(float shore, float2 worldXZ, sampler2D noiseTex) {
	shore = sqrt(shore);
	float2 noiseUV = worldXZ + _Time.y * 0.25;
	float4 noise = tex2D(noiseTex, noiseUV * 0.015);

    float distortion_advancing = noise.x * (1 - shore);
    float foam_advancing = sin((shore + distortion_advancing) * 10 - _Time.y);
    foam_advancing *= foam_advancing * shore;

    float distortion_receding = noise.y * (1 - shore);
    float foam_receding = sin((shore + distortion_receding) * 10 + _Time.y + 2);
    foam_receding *= foam_receding * 0.7;

    return max(foam_advancing, foam_receding) * shore;
}

float Waves(float2 worldXZ, sampler2D noiseTex) {
    float2 uv1 = worldXZ;
    uv1.y += _Time.y;
    float4 noise1 = tex2D(noiseTex, uv1 * 0.025);

    float uv2 = worldXZ;
    uv2.x += _Time.y;
    float4 noise2 = tex2D(noiseTex, uv2 * 0.025);

    float blendWave =
        sin((worldXZ.x + worldXZ.y) * 0.1 + (noise1.y + noise2.z) + _Time.y);
    blendWave *= blendWave;

    float waves =
        lerp(noise1.z, noise1.w, blendWave) +
        lerp(noise2.x, noise2.y, blendWave);
    return smoothstep(0.75, 2, waves);
}
#endif