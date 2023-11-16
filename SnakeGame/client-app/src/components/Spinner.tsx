interface Props {
  text: string;
  className?: string;
}

export const Spinner = (props: Props) => {
  return (
    <div
      className={`flex flex-col items-center justify-center ${props.className}`}
    >
      <div className="inline-block h-8 w-8 animate-spin rounded-full border-4 border-current border-r-transparent text-red-500 motion-reduce:animate-[spin_1.5s_linear_infinite]"></div>
      <p className="font-semibold">{props.text}</p>
    </div>
  );
};
export default Spinner;
