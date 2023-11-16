import { Link, useLocation } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { useRef, useState } from "react";
import { FormProvider, useForm } from "react-hook-form";
import Input from "../components/Input";
import {
  passwordValidation,
  usernameValidation,
} from "../utils/inputValidations";
import { ServerError } from "../shared/interfaces/ServerError";
import logo from "../assets/snake_64_bw.png";
import Button from "../components/Button";

interface State {
  isRegistrationSuccessful: boolean;
}

interface FormInput {
  username: string;
  password: string;
}

interface UserDto {
  username: string;
  password: string;
}

export const Login = () => {
  const { login } = useAuth();
  const location = useLocation();

  const methods = useForm<FormInput>();
  const [serverErrors, setServerErrors] = useState<ServerError[]>([]);
  const isLoginSuccessful = useRef(false);

  async function loginUser(credentials: UserDto) {
    return await fetch("/api/auth/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(credentials),
    })
      .then(async (res) => {
        if (!res.ok) {
          const err = await res.json();

          const errs: ServerError[] = [];
          for (const field in err) {
            const messages = err[field];

            for (const message of messages) {
              errs.push({ field, message });
              break; // only single message per field is displayed
            }
          }
          setServerErrors(errs);

          return await Promise.reject(err);
        }

        isLoginSuccessful.current = true;
        return res.text();
      })
      .then((data) => {
        return data;
      })
      .catch((e) => {
        console.error("User login fetch error: ", e);
        return null;
      });
  }

  const onSubmit = methods.handleSubmit(async (formInput) => {
    const token = await loginUser(formInput);

    if (isLoginSuccessful.current && token) {
      sessionStorage.setItem("username", formInput.username);
      methods.reset();
      login(token);
    }
  });

  return (
    <div className="flex flex-col items-center gap-5">
      <h1 className="mb-32 mt-24 text-7xl font-thin">
        <img src={logo} className="float-left mr-1" alt="S" />
        nake game
      </h1>
      {(location.state?.isRegistrationSuccessful as State) && (
        <p className="rounded-md bg-green-100 px-2 font-semibold text-green-500">
          Account registered successfully!
        </p>
      )}
      <FormProvider {...methods}>
        <form
          onSubmit={(e) => e.preventDefault()}
          noValidate
          autoComplete="off"
          className="flex flex-col items-center gap-5"
        >
          <Input {...usernameValidation} serverError={serverErrors} />
          <Input {...passwordValidation} serverError={serverErrors} />
          <Button onClick={onSubmit} text="Log In" />
        </form>
      </FormProvider>
      <p>
        Don't have an account?&nbsp;
        <Link
          to="/register"
          className="font-semibold text-blue-500 hover:underline"
        >
          Register
        </Link>
      </p>
    </div>
  );
};

export default Login;
