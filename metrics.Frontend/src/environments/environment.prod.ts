export const environment = {
  production: true,
  baseUrl: "",
  get apiUrl() : string {
    return this.baseUrl + '/api'
  }
};
