import { Component, OnInit } from '@angular/core';
import { Course } from './models';
import { HackingService } from './hacking.service';

import 'rxjs/Rx';
declare var jsHelper: any;

@Component({
    selector: 'admin-examples',
    templateUrl: 'app/admin-examples.component.html',
    providers: [ HackingService ]
})
export class AdminExamplesComponent {
    constructor(
        private hackingService: HackingService) { }

    repository: string = "defined in App_Data/Settings/ExamplesRepo.yaml";
    updateResult: string = "";
    isError: boolean = false;

    update(): void {
        var self = this;
        this.updateResult = "";
        this.isError = false;
        jsHelper.showWaitDialog();
        this.hackingService.updateExamples().subscribe(result => {
            self.updateResult = result;
            jsHelper.hideWaitDialog();
        }, error => {
            jsHelper.hideWaitDialog();
            self.isError = true;
        });
    }
}