
// Include global variables first
import "../node_modules/@spectrum-css/vars/dist/spectrum-global.css";

// Include only the scales your application needs
import "../node_modules/@spectrum-css/vars/dist/spectrum-medium.css";
//import "../node_modules/@spectrum-css/vars/dist/spectrum-large.css";

// Include only the colorstops your application needs
//import "../node_modules/@spectrum-css/vars/dist/spectrum-lightest.css";
import "../node_modules/@spectrum-css/vars/dist/spectrum-light.css";
import "../node_modules/@spectrum-css/vars/dist/spectrum-dark.css";
//import "../node_modules/@spectrum-css/vars/dist/spectrum-darkest.css";

// Include index-vars.css for all components you need
import "../node_modules/@spectrum-css/page/dist/index-vars.css";
import "../node_modules/@spectrum-css/typography/dist/index-vars.css";
import "../node_modules/@spectrum-css/icon/dist/index-vars.css";
import "../node_modules/@spectrum-css/button/dist/index-vars.css";
import "../node_modules/@spectrum-css/sidenav/dist/index-vars.css";


import "./styles.scss";
//import "./css/bootstrap/bootstrap.min.css";
// import "./css/site.css";

const loadIcons = require('loadicons');

//import * as loadIcons from "../node_modules/loadicons/index.js";

loadIcons('assets/icons/spectrum-css-icons.svg')
loadIcons('assets/icons/spectrum-icons.svg');

console.log("hello world!");