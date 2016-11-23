import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';

@Component({
    selector: 'markdown',
    template: `<div [innerHTML]="convertedData"></div>`
})
export class MarkdownComponent implements OnChanges, OnInit {
    @Input() data: string = "";
    convertedData: string;

    ngOnInit(): void {
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (this.data != null) {
            this.convertedData = marked(this.data);
        }
    }
}