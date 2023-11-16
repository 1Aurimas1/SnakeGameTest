import { FieldErrors, FieldValues } from "react-hook-form";

export function getInputErrorMessage(
  errors: FieldErrors<FieldValues>,
  name: string,
): string {
  for (const key of Object.keys(errors)) {
    if (key.includes(name)) {
      return errors[key]?.message?.toString() ?? "";
    }
  }

  return "";
}
