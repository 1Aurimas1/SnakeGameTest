import { FieldValues, RegisterOptions, useFormContext } from "react-hook-form";
import { getInputErrorMessage } from "../utils/findInputError";
import { ServerError } from "../shared/interfaces/ServerError";
import { useEffect } from "react";

interface Props {
  name: string;
  label: string;
  type: string;
  id: string;
  placeholder: string;
  validation?: RegisterOptions<FieldValues, string>;
  serverError: ServerError[];
}

const InputError = ({ message }: { message: string }) => {
  return (
    <p className="rounded-md bg-red-100 px-2 font-semibold text-red-500">
      {message}
    </p>
  );
};

export const Input = ({
  name,
  label,
  type,
  id,
  placeholder,
  validation,
  serverError,
}: Props) => {
  const {
    register,
    setError,
    formState: { errors },
  } = useFormContext();

  const inputErrors = getInputErrorMessage(errors, name);

  useEffect(() => {
    if (serverError) {
      for (const err of serverError) {
        const field = err.field.toLowerCase();
        if (field === name) {
          setError(field, {
            type: "server",
            message: err.message,
          });
        }
      }
    }
  }, [serverError]);

  return (
    <div className="flex w-72 flex-col gap-2">
      <div>
        <label htmlFor={id} className="font-semibold capitalize">
          {label}
        </label>
        {inputErrors && <InputError message={inputErrors} key={inputErrors} />}
      </div>
      <input
        id={id}
        type={type}
        className="rounded-md border-2 p-3 font-medium placeholder:opacity-50"
        placeholder={placeholder}
        {...register(name, validation)}
      />
    </div>
  );
};

export default Input;
