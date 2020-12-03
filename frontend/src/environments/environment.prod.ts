export const environment = {
  production: true,
  baseUrl: "",
  identityUrl: 'http://metrics-identity',
  get apiUrl() : string {
    return this.baseUrl + '/api'
  }
};
