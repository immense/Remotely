export class CommandLineParameter {
    constructor(name: string, value: string) {
        this.Name = name;
        this.Value = value;
    }
    Name: string;
    Value: string;
}