using UnityEngine;

public static class AudioClipVisualizer {
	public static Texture2D PaintWaveformSpectrum(AudioClip audio, float saturation, int height, Color col, int? width = null) {
		width = width == null ? Mathf.CeilToInt(audio.length * 100f) : width;

		Texture2D tex = new Texture2D(width.Value, height, TextureFormat.RGBA32, false);
		float[] samples = new float[audio.samples];
		float[] waveform = new float[width.Value];
		audio.GetData(samples, 0);
		int packSize = (audio.samples / width.Value) + 1;
		int s = 0;
		for(int i = 0; i < audio.samples; i += packSize) {
			waveform[s] = Mathf.Abs(samples[i]);
			s++;
		}

		for(int x = 0; x < width; x++) {
			for(int y = 0; y < height; y++) {
				tex.SetPixel(x, y, Color.black);
			}
		}

		for(int x = 0; x < waveform.Length; x++) {
			for(int y = 0; y <= waveform[x] * ((float)height * .75f); y++) {
				tex.SetPixel(x, (height / 2) + y, col);
				tex.SetPixel(x, (height / 2) - y, col);
			}
		}
		tex.Apply();

		return tex;
	}
}