import {IGuidData} from "./IGuidData";

export interface IActionData extends IGuidData {
  name: string;
  action: string;
  target: string;
}
