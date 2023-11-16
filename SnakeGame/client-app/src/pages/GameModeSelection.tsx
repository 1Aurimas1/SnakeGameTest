import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { GameMode } from "../shared/constants/GameMode";
import Button from "../components/Button";
import ContentWrapper from "../components/ContentWrapper";

interface ModeSelection {
  title: string;
  mode: GameMode;
}

const modeSelections: ModeSelection[] = [
  { title: "Practice", mode: GameMode.SOLO },
  {
    title: "Duel",
    mode: GameMode.DUEL,
  },
];

const GameModeSelection = () => {
  const { user } = useAuth();
  const navigate = useNavigate();

  const handleSelection = (
    e: React.MouseEvent<HTMLButtonElement, MouseEvent>,
    selectedMode: GameMode,
  ) => {
    e.preventDefault();
    navigate("/play", { state: { selectedMode, user } });
  };

  return (
    <ContentWrapper title="Select game mode">
      {modeSelections.map((selection) => (
          <Button
            key={selection.mode}
            onClick={(e) => handleSelection(e, selection.mode)}
            text={selection.title}
            className="m-1 border-2 border-black font-semibold"
          />
      ))}
    </ContentWrapper>
  );
};

export default GameModeSelection;
