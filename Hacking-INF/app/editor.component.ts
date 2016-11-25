import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';

@Component({
    selector: 'editor',
    template: `<div [innerHTML]="code"></div>`
})
export class MarkdownComponent implements OnChanges, OnInit {
    @Input() code: string = "";

    ngOnInit(): void {
    }

    ngOnChanges(changes: SimpleChanges): void {
    }
}