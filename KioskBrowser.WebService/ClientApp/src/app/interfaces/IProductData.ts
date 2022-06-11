import {IGuidData} from "./IGuidData";

export interface IProductData extends IGuidData {
  name: string,
  group: string,
  sortIndex: number,
  totalItems: number
}
