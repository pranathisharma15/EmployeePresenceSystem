import axios from "axios";

const API = axios.create({
  baseURL: "https://localhost:7184/api",
});

const TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImV4cCI6MTc3NDY3OTM0MiwiaXNzIjoib2ZmaWNlLWFwcCIsImF1ZCI6Im9mZmljZS1hcHAifQ.yME8DC4R_16jWjj9cWAv5A8mvkjQeM_XLpz3u7yE-2k";

API.interceptors.request.use((req) => {
  req.headers.Authorization = `Bearer ${TOKEN}`;
  return req;
});

export default API;