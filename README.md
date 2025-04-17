# Moser's Game of Life

## Overview

A modern implementation of Conway's Game of Life cellular automaton. This project simulates the evolution of a grid of cells based on a set of simple rules, resulting in complex emergent behaviors.

## Features

### Simulation Controls
- Start, pause, and resume the simulation
- Generate new random configurations
- Adjust cell painting modes (alive or dead cells)
- Visual cell trails option to see the history of cell movements

### Color Behaviors
- **Default Mode**: Traditional green cells on black background
- **Majority Color**: Cells adopt the most common color of their neighbors
- **Average Color**: Cells blend the colors of their neighbors

### Ruleset Management
- Create, save, and delete custom rulesets
- Real-time rule editing with B/S notation display
- Detection of duplicate rulesets

### Interactive Cell Painting
- Paint alive or dead cells directly on the grid
- Dynamic color generation for painted cells
- Cell state toggling with mouse interaction

## Standard Game Rules
The classic Conway's Game of Life follows these rules:
1. Any live cell with fewer than two live neighbors dies (underpopulation)
2. Any live cell with two or three live neighbors lives on to the next generation
3. Any live cell with more than three live neighbors dies (overpopulation)
4. Any dead cell with exactly three live neighbors becomes a live cell (reproduction)

## Available Predefined Rulesets

| Name                   | Notation            | Description                                 | Reference |
|------------------------|---------------------|---------------------------------------------|-----------|
| Conway's Game of Life  | B3/S23              | The classic cellular automaton              | [Wikipedia](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life) |
| HighLife               | B36/S23             | Supports replicators                        | [Wikipedia](https://en.wikipedia.org/wiki/Highlife_(cellular_automaton)) |
| Day & Night            | B3678/S34678        | Symmetrical rule                            | [LifeWiki](https://conwaylife.com/wiki/Day_and_Night) |
| Replicator             | B1357/S1357         | Creates replicating patterns                | [LifeWiki](https://conwaylife.com/wiki/Replicator) |
| Seeds                  | B2/S                | Fast growth, chaotic                        | [LifeWiki](https://conwaylife.com/wiki/Seeds) |
| Life without death     | B3/S012345678       | Everything lives forever                    | [LifeWiki](https://conwaylife.com/wiki/Life_without_Death) |
| 2x2                    | B36/S245            | Blocks, emulates rule 90                    | [LifeWiki](https://conwaylife.com/wiki/2x2) |
| Assimilation           | B345/S5             | Assimilates patterns                        | [LifeWiki](https://conwaylife.com/wiki/Assimilation) |
| Isolated Birth         | B1/S012345678       | Give birth when isolated (filling pattern)  | [LifeWiki](https://conwaylife.com/wiki/Isolated_Birth) |
| Maze                   | B34/S34             | Tends to form stable mazes                  | [LifeWiki](https://conwaylife.com/wiki/Maze) |
| Coagulations           | B38/S23             | Forms growing blobs                         | [LifeWiki](https://conwaylife.com/wiki/Coagulations) |
| Diamoeba               | B3/S0123456         | Chaotic amoeba-like growth                  | [LifeWiki](https://conwaylife.com/wiki/Diamoeba) |
| Anneal                 | B2/S345678          | Melts patterns together                     | [LifeWiki](https://conwaylife.com/wiki/Anneal) |
| Long Life              | B3/S12345           | Long-living structures                      | [LifeWiki](https://conwaylife.com/wiki/Long_Life) |
| Gnarl                  | B25/S4              | Tree-like growth                            | [LifeWiki](https://conwaylife.com/wiki/Gnarl) |
| Stains                 | B357/S1358          | Forms stain-like patterns                   | [LifeWiki](https://conwaylife.com/wiki/Stains) |
| Fill                   | B012345678/S012345678 | Everything fills instantly                | [LifeWiki](https://conwaylife.com/wiki/Fill) |

## Technical Details
- Built with C# and WPF
- Uses parallel processing for efficient grid updates
- Implements customizable cellular automaton rules
- JSON-based ruleset persistence

## Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

## License
This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments
- John Conway for inventing the original Game of Life
- jaxry for his visualization of colorful-life

## Inspirations & Further Reading
- [Conway's Game of Life with Different Rules](https://dev.to/lexjacobs/conways-game-of-life-with-different-rules-13l0)
- [Conway's Game of Life Variations](https://www.algoritmarte.com/conways-game-of-life-variations/)
- [Life-like Cellular Automaton](https://en.wikipedia.org/wiki/Life-like_cellular_automaton)
