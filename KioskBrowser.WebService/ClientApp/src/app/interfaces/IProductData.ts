import {IGuidData} from "./IGuidData";

export interface IProductData extends IGuidData {
  title: string,
  group: string,
  sortIndex: number,
  totalItems: number
}
