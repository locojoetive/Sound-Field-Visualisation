# Sound Field Visualisation for HoloLens

### 1. Origin

This project was a research project at the [Acoustic Lab](http://www.acoust.ias.sci.waseda.ac.jp/) at Waseda University in Tokyo.
It extended on the research of [Kataoka et. al.](http://contents.acoust.ias.sci.waseda.ac.jp/publications/ACM/2018/SIGGRAPHASIA-kataoka-2018Dec.pdf) about [*Real-time Measurement and Display System of 3D Sound Intensity Map using Optical See-Through Head Mounted Display*](https://www.youtube.com/watch?v=xOXdfPGw3rA).

With their help and by using their collected data I developed a pseudo algorithm, that localized a sound source and visualized the recorded sound in circular waves.
The results are visualized [here](https://www.youtube.com/playlist?list=PL16zWJuXWCiwZhqUBcDZxl4h0IGznKrt-).

## 1.1. Requirements

Unity [VERSION]
Holotoolkit [VERSION]

optional
HoloLens 1

### 2. Pseudo Sound Source Localization

For the following approach I assume that the recorded sound is coming from a single source, that each recorded data point is measuring the same sound (frequency and )
Each recorded data point is classified as a Particle and has two relevant properties: direction and sound intensity.
```
struct Particle {
	position: Vector3
	intensity: Vector3
}

```
```position``` is the particle's world position relative to a given reference point.
The direction of the property ```intensity``` defines the direction in which the recorded sound travelled, whereas the vector's length is determined by the sound intensity.

```
Function 
Data: particles: List<Particles>,
	with particles[i].position: Vector3
	and particles[i].intensity: Vector3
Result: center: Vector3
	centerCandidates : List <Vector3>
	for i ← 0 to particles.length by 1 do
		for j ← 0 to particles.length by 1 do
			if i != j then
				centerCandidate ← SkewDistanceCenter(
					particles[i].position + particles[i].intensity,
					particles[j].position + particles[j].intensity
				)
		 		centerCandidates.Add(centersCandidate)
			end
		end
	end
 	return AveragePosition(centerCandidates)
```

### 3. Adjustment of Sound Particles

For each pair of two distinct particles I calculate the closest point between their direction vectors, which is the center between these two vectors.
For a number of n particles I then get n^2 center candidates.
By calculating the avarage position of these n^2 positions, I determine a hypothetical sound source, named ```center```.

The intensity vector is then updated by using the following steps:
1. Determine its new ```direction = particle.position - center```
2. Calculating the angle ```α``` between ```direction``` and  ```particle.intensity``` 
3. Adjust the new intensity level ```particle.intensity = ((180−α)/180) * particle.intensity```

Hereby, I tried to consider (ausbügeln) that sound could be scattered, reflected, or refracted and adjust the particles intensity to the new ```center```.

### 4. Particle Displacement

I assume that the sound propagates spherically.
Therefore, we represent the sound propagation as a sphere with ```center``` as its center and with its vertices displaced depending on the adjusted particles.
The bigger the angle between a particle's intensity and its closest sphere vertex, the smaller its impact on the vertex displacement.
The result is a deformed sphere that represents the sound propagation.


### 5. Animation

The deformed sphere's vertices are passed to a shader that generates a 2D plane.
This plane then renders an intersection (Querschnitt) of the sphere and illustrates the sound propagation as wave impulses.
