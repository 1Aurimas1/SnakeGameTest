interface Props {
  name: string;
  value: number;
}

export const Stat = (props: Props) => {
  return (
    <div>
      <p className="inline-block">{props.name}</p>
      <p className="float-right ml-1 inline-block">{props.value}</p>
    </div>
  );
};

export default Stat;
