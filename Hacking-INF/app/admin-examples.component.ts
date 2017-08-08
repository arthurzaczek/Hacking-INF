import { Component, OnInit } from '@angular/core';
import { Course } from './models';
import { HackingService } from './hacking.service';

 import 'rxjs/Rx';

@Component({
    selector: 'admin-examples',
    templateUrl: 'app/admin-examples.component.html',
    providers: [ HackingService ]
})
export class AdminExamplesComponent {
    constructor(
        private hackingService: HackingService) { }

    repository: string = "defined in Web.config";
    updateResult: string = "";

    update(): void {
        var self = this;
        this.hackingService.updateExamples().subscribe(result => {
            self.updateResult = result;
        });
    }
}