interface Props {
  onClick: React.MouseEventHandler<HTMLButtonElement> | undefined;
  className?: string;
  text: string;
}

export const Button = (props: Props) => {
  return (
    <button
      onClick={props.onClick}
      className={`w-28 rounded bg-gray-500 py-2 text-white hover:bg-gray-200 hover:text-black active:bg-red-500 active:text-black ${props.className}`}
    >
      {props.text}
    </button>
  );
};

export default Button;
