import * as Yup from "yup";

const shareStorySchema = Yup.object().shape({
  name: Yup.string()
    .required("Name is required!")
    .min(2, "Name must be at least 2 characters!")
    .max(50, "Name can be at most 50 characters!"),
  email: Yup.string()
    .required("Email is required!")
    .email("Invalid email address!"),
  story: Yup.string()
    .required("Story is Required!")
    .max(3000, "Story can be at most 3000 characters!"),

});

export default shareStorySchema;
