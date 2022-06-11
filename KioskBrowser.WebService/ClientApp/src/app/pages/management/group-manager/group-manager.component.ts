import { Component, OnInit } from '@angular/core';
import {DataHubService, IGroupData} from "../../../hubs/data-hub.service";
import {CdkDragDrop} from "@angular/cdk/drag-drop";

@Component({
  selector: 'app-group-manager',
  templateUrl: './group-manager.component.html',
  styleUrls: ['./group-manager.component.css']
})
export class GroupManagerComponent implements OnInit {
  public groups: IGroupData[] = [];
  public currentGroup: IGroupData = {id: '', name: '', sortIndex: 0};

  constructor(public data: DataHubService) { }

  ngOnInit(): void {
    this.data.connectionReady.subscribe(() => {
      console.log('Ready');
      this.data
        .allGroups()
        .then(x => this.setGroupCollection(x));

      this.data.groupDataChange.subscribe(x => this.setGroupCollection(x));
    });
  }

  public setGroupCollection(groups: IGroupData[]) {
    this.groups = groups
      .sort((a, b) => a.sortIndex > b.sortIndex ? 1 : -1);

    for (let i = 0; i < this.groups.length; i++) {
      this.groups[i].sortIndex = i;
    }

    console.log(this.groups);
  }

  public resetCurrentGroup() {
    this.currentGroup = {id: '', name: '', sortIndex: 0};
  }

  public saveCurrentGroup() {
    this.data.addEditGroup(this.currentGroup).finally(() => {
      alert('Done');
    });
    this.resetCurrentGroup();
  }

  public removeGroup(group: IGroupData) {
    if (!confirm('Are you sure?')) {
      return;
    }
    this.data.removeGroup(group).finally(() => alert('Done'));
  }

  public groupDrop(event: CdkDragDrop<any, any>) {
    this.groups[event.previousIndex].sortIndex = event.currentIndex;
    this.groups[event.currentIndex].sortIndex = event.previousIndex;

    this.data.addEditGroup(this.groups[event.previousIndex]).finally(() => {});
    this.data.addEditGroup(this.groups[event.currentIndex]).finally(() => {});
  }

}
