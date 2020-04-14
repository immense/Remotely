import { DynamicDto } from "../DynamicDto.js";

export interface ScreenDataDto extends DynamicDto {
    DisplayNames: string[];
    SelectedScreen: string;
}