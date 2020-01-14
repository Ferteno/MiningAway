/* Author: Ali Sher
 * File Name: TerrainBuilder.cs
 * Creation Date: 2018-01
 * Modification Date: 2018-01-22
 * Description: Builds the terrain, and loads chunks
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MiningAwayISU
{
    class TerrainBuilder
    {
        //values for width and height
        int width;
        int height;

        //constant int for the length and width of 3 chunks
        public const int LENGTH_OF_CHUNKS = 150;

        //perlin algorithm obj.
        FastNoise noiseGen;

        //array of tile values
        int[,] tileValues;

        //rng to get the value for rng heights
        Random rng = new Random();
        
        //seed value used to randomize
        int seed;

        //roughnesses for diff generations
        double underGroundRoughness = 0.01;
        double plainsRoughness = 0.002;
        double hillsRoughness = 0.007;

        //numbers to track the block being loaded in
        int curTileHeight;
        int curTileWidth;

        //ints to track the last block generated in each direction
        public int lastTileDown;
        public int lastTileUp;
        public int lastTileLeft = 250;
        public int lastTileRight;

        //integers to track which sides are being generated
        public const int GEN_RIGHT =0;
        public const int GEN_LEFT = 1;
        //const int GEN_ =

        //size of chunks
        const int CHUNK_W = 50;
        const int CHUNK_H = 50;

        //Default constructor
        public TerrainBuilder(int terrainWidth, int terrainHeight)
        {
            //pass in parameters
            width = terrainWidth;
            height = terrainHeight;

            //rng seed
            seed = rng.Next(-10000, 10000);

            //set the seed for the noise gen
            noiseGen = new FastNoise(seed);
            noiseGen.SetFractalOctaves(10);
            noiseGen.SetGradientPerturbAmp(5);
            noiseGen.SetFractalLacunarity(3);
            noiseGen.SetFractalGain(0.4);
            noiseGen.SetNoiseType(FastNoise.NoiseType.PerlinFractal);

            //initialize multi-dimensional array
            tileValues = new int[height, width];
        }

        //Pre: THE STARTING TILE, AND THE side being generated
        //Post: Returns an array of ints
        //Desc: Iterates through to get different float values
        public int[,] Build(int startingTileWidth, int startingTileHeight, int genType)
        {

            //generate 3 chunks at a time
            for (int c = 0; c < width / CHUNK_W; c++)
            {
                //reset current tile height
                curTileHeight = 0;

                //generate 3 chunks laterally at a time
                for (int d = 0; d < width / CHUNK_W; d++)
                {
                    //get a random number for the chunks generation
                    int chunkGen = rng.Next(0, 4);

                    //change the values for the noise gen
                    noiseGen.SetFrequency(underGroundRoughness);
                    noiseGen.SetFractalOctaves(10);
                    noiseGen.SetGradientPerturbAmp(5);
                    noiseGen.SetFractalLacunarity(3);
                    noiseGen.SetFractalGain(0.4);
                    noiseGen.SetNoiseType(FastNoise.NoiseType.PerlinFractal);

                    //generate underground if the chunk is greater than 3/4 the height, else generate the surface
                    if (curTileHeight > (2 * height / 3))
                    {
                        //loop through from the current tile, and go through till the end of the chunk width
                        for (int i = curTileWidth; i < (curTileWidth + CHUNK_W); i++)
                        {
                            //generate the shape of the bottom (between dirt and rock layers)
                            double bottom = noiseGen.GetNoise(0, (double)(i));
                            bottom += 1;
                            int h = (int)(Math.Round(bottom * (height / 8)));

                            //loop through from the current tile, and go through till the end of the chunk height
                            for (int j = curTileHeight - h; j < (curTileHeight + CHUNK_H); j++)
                            {
                                //if the chunkGen is a 1, generate caves, else, generate solid rock
                                if (chunkGen <= 2 && (curTileHeight >  3 * height / 4))
                                {
                                    //generate a random number through perlin noise
                                    double num = noiseGen.GetNoise((double)((i)), (double)(j));

                                    //if the number is greater than -0.1, set the block to stone
                                    if (num > -0.1)
                                    {
                                        tileValues[j, i] = Tiles.STONE_BLOCK;
                                    }
                                    //else, set the block to air
                                    else
                                    {
                                        tileValues[j, i] = Tiles.AIR_BLOCK;
                                    }
                                }
                                else
                                {
                                    //spawn chance of an iron ore 
                                    int oreSpawn = rng.Next(1, 10);

                                    //if the number pulled is 1, spawn an iron block, else spawn a stone
                                    if (oreSpawn == 1)
                                    {
                                        tileValues[j, i] = Tiles.IRON_BLOCK;
                                    }
                                    else
                                    {
                                        tileValues[j, i] = Tiles.STONE_BLOCK;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //randomly choose between plains and hills surface smoothness
                        double surfaceRoughness;

                        //set the roughness for the chunk
                        if (rng.Next(1, 5) == 1)
                        {
                            surfaceRoughness = hillsRoughness;
                        }
                        else
                        {
                            surfaceRoughness = plainsRoughness;
                        }

                        //ABOVEGROUND
                        for (int i = curTileWidth; i < (curTileWidth + CHUNK_W); i++)
                        {
                            //set the smoothness to the previously decided double
                            noiseGen.SetFrequency(surfaceRoughness);

                            double top = noiseGen.GetNoise(0, (double)(i));

                            //z is generated from {-1, 1}, so fix it to be between 0 and 1
                            top += 1;

                            //multpily the z value with the heightMultiplier (height / 2) and round it
                            int h = height - (int)(Math.Round(top * (height / 2)));

                            for (int j = curTileHeight; j < (curTileHeight + CHUNK_H); j++)
                            {
                                //depending on the height, set the block to right type
                                if (j > h)
                                {
                                    tileValues[j, i] = Tiles.DIRT_BLOCK;
                                }
                                else if (j == h)
                                {
                                    tileValues[j, i] = Tiles.GRASS_BLOCK;
                                }
                                else
                                {
                                    tileValues[j, i] = Tiles.AIR_BLOCK;
                                }
                            }
                        }
                    }

                    //GENERATE ROCKS AND TUNNELS IN DIRT, TREES AND STRUCTURES
                    for (int genLoop = 1; genLoop <= 4; genLoop++)
                    {
                        //loop through the width
                        for (int i = curTileWidth; i < (curTileWidth + CHUNK_W); i++)
                        {
                            //loop through the height
                            for (int j = curTileHeight; j < (curTileHeight + CHUNK_H); j++)
                            {
                               // generate rocks in the soil
                                if (genLoop == 1)
                                {
                                    if (tileValues[j, i] == Tiles.DIRT_BLOCK)
                                    {
                                        noiseGen.SetFractalOctaves(10);
                                        noiseGen.SetFrequency(0.6);
                                        noiseGen.SetFractalLacunarity(3);
                                        noiseGen.SetFractalGain(0.4);

                                        double num = noiseGen.GetNoise((double)((i)), (double)(j));

                                        if (num < -0.155)
                                        {
                                            //spawn chance of an iron ore 
                                            int oreSpawn = rng.Next(1, 25);

                                            //if the number pulled is 1, spawn an iron block, else spawn a stone
                                            if (oreSpawn == 1)
                                            {
                                                tileValues[j, i] = Tiles.IRON_BLOCK;
                                            }
                                            else
                                            {
                                                tileValues[j, i] = Tiles.STONE_BLOCK;
                                            }
                                        }
                                    }
                                }
                                //generate the tunnels in the soil
                                else if (genLoop == 2)
                                {
                                    if (tileValues[j, i] == Tiles.DIRT_BLOCK || tileValues[j, i] == Tiles.GRASS_BLOCK)
                                    {
                                        noiseGen.SetFrequency(0.09);
                                        noiseGen.SetCellularReturnType(FastNoise.CellularReturnType.Distance2Div);
                                        noiseGen.SetNoiseType(FastNoise.NoiseType.Cellular);

                                        double num = noiseGen.GetNoise((double)((i)), (double)(j));

                                        if (num > 0.87)
                                        {
                                            tileValues[j, i] = Tiles.AIR_BLOCK;
                                        }
                                    }
                                }
                                //GENERATE TREES
                                else if (genLoop == 3)
                                {
                                    //rng for tree spawn (1 in 8 chance)
                                    int treeChance = rng.Next(1, 9);

                                    //f "1" is rolled, spawn a tree of random height
                                    if (treeChance < 3 && tileValues[j, i] == 1 && j > 20)
                                    {
                                        //random number for tree height (5 blocks - 20 blocks)
                                        int treeH = rng.Next(5, 21);

                                        //loop through to create tree
                                        for (int k = 1; k <= treeH; k++)
                                        {
                                            //check if there is a tree right beside the current one
                                            if (i > 1 && !(tileValues[j - k, i - 1] == Tiles.LOG_BLOCK))
                                            {
                                                tileValues[j - k, i] = Tiles.LOG_BLOCK;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                //GENERATE STRUCTURES
                                else if (genLoop == 4)
                                {
                                    //number to represent chance of generating a structure
                                    int structureGen = rng.Next(1, 100);

                                    //if the chance for a wooden structure is satisfied, and the current block is grass, build a wood structure
                                    //make sure the values do not go out of bounds
                                    if (j - 6 > 0 && i + 6 < width)
                                    {
                                        if (structureGen <= 1 && tileValues[j, i] == Tiles.GRASS_BLOCK)
                                        {
                                            //hard code for wooden structure
                                            //Left Side
                                            tileValues[j - 1, i] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 2, i] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 3, i] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 4, i] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 5, i] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 6, i] = Tiles.WOOD_BLOCK;
                                            //Right Side
                                            tileValues[j - 1, i + 6] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 2,  i + 6] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 3, i + 6] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 4, i + 6] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 5, i + 6] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 6, i + 6] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 7, i + 6] = Tiles.WOOD_BLOCK;
                                            //Bottom
                                            tileValues[j - 1, i + 1] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 1, i + 2] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 1, i + 3] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 1, i + 4] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 1, i + 5] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 1, i + 6] = Tiles.WOOD_BLOCK;
                                            //Top
                                            tileValues[j - 7, i] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 7, i + 1] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 7, i + 2] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 7, i + 3] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 7, i + 4] = Tiles.WOOD_BLOCK;
                                            tileValues[j - 7, i + 5] = Tiles.WOOD_BLOCK;

                                            //all tiles below structure are solid
                                            tileValues[j, i + 1] = Tiles.GRASS_BLOCK;
                                            tileValues[j, i + 2] = Tiles.GRASS_BLOCK;
                                            tileValues[j, i + 3] = Tiles.GRASS_BLOCK;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //update the curTileHeight
                    curTileHeight += CHUNK_H;
                }

                //update the curTileWidth
                curTileWidth +=  CHUNK_W;
            }

            //update the last generated block after generation is complete
            switch (genType)
            {
                case GEN_RIGHT:
                    {
                        lastTileRight = curTileWidth;
                        break;
                    }
                case GEN_LEFT:
                    {
                        lastTileLeft = curTileWidth;
                        break;
                    }
            }

            //return the list of points
            return tileValues;
        }

        //Pre: integer representing the last tile widthwise of the chunk
        //Post: Return a vector 2
        //Desc: Return a vector 2 for a good location for the companion
        public Vector2 GetCompanionLoc(int lastTileWidth)
        {
            //find a grass tile at a random length from the beginning to the end of the chunk and add it to the location
            int xTile = rng.Next(lastTileWidth - LENGTH_OF_CHUNKS, lastTileWidth);

            //Vector 2 location of the companion
            Vector2 compLoc = new Vector2();

            //integer of the companion height
            int companionHeight = 30;

            //find the grass or dirt block at the xTile
            for (int i = 0; i < height; i++)
            {
                //store the location in compLoc
                if (tileValues[i, xTile] == Tiles.GRASS_BLOCK || tileValues[i, xTile] == Tiles.DIRT_BLOCK)
                {
                    compLoc = new Vector2(Tiles.TILE_SIZE * xTile, (i * Tiles.TILE_SIZE) - companionHeight);
                }
            }

            return compLoc;
        }

    }
}
