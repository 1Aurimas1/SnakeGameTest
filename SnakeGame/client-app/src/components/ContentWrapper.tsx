import { ReactNode } from "react";

interface Props {
  title: string;
  children: ReactNode;
}

export const ContentWrapper = (props: Props) => {
  return (
    <div className="m-auto flex items-center justify-center">
      <div className="flex flex-col items-center justify-center border border-black bg-gray-200 p-5">
        <h1 className="mx-5 mb-5 border-b border-black pb-3 text-xl font-bold">
          {props.title}
        </h1>
        {props.children}
      </div>
    </div>
  );
};

export default ContentWrapper;
