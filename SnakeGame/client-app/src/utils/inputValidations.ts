const isValidEmail = (email: string): boolean => {
  if (email.includes(" ")) return false;

  const emailPattern =
    /(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/;
  const re = RegExp(emailPattern);

  return re.test(email);
};

export const usernameValidation = {
  label: "username",
  id: "username",
  name: "username",
  type: "text",
  placeholder: "username...",
  validation: {
    required: {
      value: true,
      message: "This field can't be empty",
    },
    maxLength: {
      value: 20,
      message: "Should contain a maximum of 20 characters",
    },
  },
};

export const emailValidation = {
  label: "email address",
  id: "email",
  name: "email",
  type: "email",
  placeholder: "email...",
  validation: {
    required: {
      value: true,
      message: "This field can't be empty",
    },
    validate: (email: string) =>
      isValidEmail(email) || "Please enter a valid email.",
  },
};

export const passwordValidation = {
  label: "password",
  id: "password",
  name: "password",
  type: "password",
  placeholder: "password...",
  validation: {
    required: {
      value: true,
      message: "This field can't be empty",
    },
    minLength: {
      value: 6,
      message: "Should contain atleast 6 characters",
    },
  },
};

export const passwordConfirmationValidation = {
  label: "confirm password",
  id: "passwordConfirmation",
  name: "passwordConfirmation",
  type: "password",
  placeholder: "confirm password...",
  validation: {
    required: {
      value: true,
      message: "This field can't be empty",
    },
    minLength: {
      value: 6,
      message: "Should contain atleast 6 characters",
    },
    validate: {},
  },
};
