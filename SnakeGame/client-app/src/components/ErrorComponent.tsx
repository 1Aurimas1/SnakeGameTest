import { Link } from "react-router-dom";

interface Props {
  title: string;
  helperMessage: string;
  redirectPath?: string;
  redirectMessage?: string;
}

export const ErrorComponent = (props: Props) => {
  return (
    <div className="flex flex-col items-center justify-center gap-5">
      <h1 className="text-4xl font-semibold capitalize">{props.title}</h1>
      <p className="text-gray-500">{props.helperMessage}</p>
      <Link
        to={props.redirectPath || "/"}
        className="mt-5 font-semibold text-blue-500 hover:underline"
      >
        {props.redirectMessage || "Go to home page"}
      </Link>
    </div>
  );
};

export default ErrorComponent;
