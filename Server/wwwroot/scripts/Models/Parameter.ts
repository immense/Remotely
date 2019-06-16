/**A parameter definition for Command Completion. */
export class Parameter {
    constructor(name:string, summary:string, parameterType:string) {
        this.Name = name;
        this.Summary = summary;
        this.ParameterType = parameterType;
    }

    Name: string;
    Summary: string;
    ParameterType: string;
}