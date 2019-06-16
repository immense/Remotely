import { EncodeForHTML } from "../Utilities.js";
export class ConsoleCommand {
    constructor(name, parameters, summary, syntax, extendedHelp, callback) {
        this.Name = name;
        this.Parameters = parameters;
        this.Summary = summary;
        this.Syntax = syntax;
        this.ExtendedHelp = extendedHelp;
        this.Callback = callback;
    }
    get FullHelp() {
        if (this.ExtendedHelp) {
            var fullHelp = this.PartialHelp.substring(0, this.PartialHelp.lastIndexOf("</div>"));
            fullHelp += "<br><br><span class='extended-help'>Extended Help:</span><br>" + this.ExtendedHelp + "</div>";
            return fullHelp;
        }
        else {
            return this.PartialHelp;
        }
    }
    get PartialHelp() {
        var partialHelp = `<div class='help-wrapper'>
                            <div>
                                <span class="text-primary">Summary: </span>
                                ${EncodeForHTML(this.Summary)}
                            </div>
                            <div>
                                <span class="text-success">Syntax: </span>
                                <span class="label label-default code">${EncodeForHTML(this.Syntax).trim()}</span>
                            </div>`;
        if (this.Parameters.length > 0) {
            partialHelp += "<br>";
            partialHelp += `<div class='text-info'>Parameters:</div> <div>`;
            for (var i = 0; i < this.Parameters.length; i++) {
                var paramText = "";
                if (this.Parameters[i].ParameterType) {
                    paramText = ` [${EncodeForHTML(this.Parameters[i].ParameterType)}]`;
                }
                partialHelp += `<div>-${EncodeForHTML(this.Parameters[i].Name)}${EncodeForHTML(paramText)}: ${EncodeForHTML(this.Parameters[i].Summary).trim()}</div>`;
            }
            partialHelp += "</div>";
        }
        partialHelp += "</div>";
        return partialHelp;
    }
    Execute(parameters) {
        var paramDictionary = {};
        parameters.forEach(x => {
            paramDictionary[x.Name.toLowerCase()] = x.Value;
        });
        this.Callback(parameters, paramDictionary);
    }
}
//# sourceMappingURL=ConsoleCommand.js.map