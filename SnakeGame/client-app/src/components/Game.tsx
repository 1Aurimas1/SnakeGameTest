import { useState } from "react";
import { GameDto, Point } from "../shared/interfaces/GameDto";

interface Props {
  grid: Grid;
  state: GameDto;
  isControllable: boolean;
}

interface Grid {
  rows: number;
  columns: number;
}

const CellType = {
  SNAKE_HEAD: 0,
  SNAKE_BODY: 1,
  FOOD: 2,
  NONE: 3,
} as const;
type CellType = (typeof CellType)[keyof typeof CellType];

const paintCell = (cell: CellType): string => {
  let cellColor;

  if (cell === CellType.SNAKE_BODY) {
    cellColor = "bg-gray-500";
  } else if (cell === CellType.SNAKE_HEAD) {
    cellColor = "bg-black";
  } else if (cell === CellType.FOOD) {
    cellColor = "bg-red-500";
  } else {
    cellColor = "bg-gray-200";
  }

  return cellColor;
};

const Game = (props: Props) => {
  const grid: Grid = props.grid;
  const isControllable = props.isControllable;

  const gameState: GameDto = props.state;
  const snake = gameState.snakeParts;
  const playerName = gameState.playerName;

  const [food, setFood] = useState<Point | undefined>();
  const [score, setScore] = useState<number>();

  if (gameState.food != food && gameState.food != null) {
    setFood(gameState.food);
    setScore(gameState.score);
  }

  const determineCellType = (x: number, y: number) => {
    let cell: CellType = CellType.NONE;

    for (let i = 0; i < snake.length; i++) {
      const isSnake = snake[i].x === x && snake[i].y === y;
      if (i === 0 && isSnake) {
        cell = CellType.SNAKE_HEAD;
      } else if (isSnake) {
        cell = CellType.SNAKE_BODY;
      }
    }

    if (food!.x === x && food!.y === y) {
      cell = CellType.FOOD;
    }

    return cell;
  };

  // IDEA: pre-allocate grid?
  return (
    <div className="m-10 flex flex-col items-center justify-center gap-5">
      {snake.length > 0 && food && (
        <>
          <div className="flex w-full flex-row justify-between">
            <p className="font-semibold">Name: {playerName}</p>
            <p className="font-semibold">Score: {score}</p>
          </div>

          <div
            id="game-board"
            className={`grid max-h-[400px] max-w-[800px] grid-cols-12 gap-1 ${
              isControllable && "shadow-custom shadow-red-500"
            }`}
          >
            {Array.from(
              { length: grid.rows },
              (_, idx) => grid.rows - 1 - idx,
            ).map((y, _) =>
              Array.from({ length: grid.columns }).map((_, x) => {
                const cell = determineCellType(x, y);
                const cellColor = paintCell(cell);

                return (
                  <div
                    key={`${x}-${y}`}
                    className={cellColor}
                    style={{
                      width: "20px",
                      height: "20px",
                    }}
                  />
                );
              }),
            )}
          </div>
        </>
      )}
    </div>
  );
};

export default Game;
