# README

## Contents

- About
- Install
- Use

## About

Water for Unity

Features:

- Made with Shader Graph.
- Supports Caustic Maps, Flow Maps, and Normal Maps.
- Uses custom lighting system to give the best visual results.
- Includes 45 Textures that you can mix-and-match to create over 3000 combinations of water.
- You can control where the Caustics and Tint appear using the Vertex Colors of the mesh.

Configurable Parameters:

- Color
- Reflection Sharpness
- Caustics Map (+ Tiling + Offset)
- Caustics Speed
- Caustics Color
- Flow Map (+ Tiling + Offset)
- Flow Map Speed
- Flow Map Intensity
- Normal Map (+ Tiling + Offset)
- Normal Map Strength
- Normal Map Speed
- Tint Color

## Install

1. git clone to /Packages/ folder

## Use

1. Create a new Material
2. Change Shader to OccaSoftware/Water/Opaque
3. Apply to plane
4. Configure material

### Tint and Caustic Masking

Use a tool like Polybrush to paint the vertices to use the Tint and Caustics mask features.

- Caustics appear when the Vertex Color R Channel is 1.
- The tint color is used when the Vertex Color G Channel is 0.

### Reflections

Bake a reflection probe near the water to ensure it has reflections.
