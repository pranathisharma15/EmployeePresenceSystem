import API from "./api";

export const loginUser = (data) => API.post("/auth/login", data);

export const getEmployees = (filters) =>
  API.get("/employee", { params: filters });