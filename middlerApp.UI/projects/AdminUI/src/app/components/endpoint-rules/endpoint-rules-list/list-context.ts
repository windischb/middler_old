export class ListContext<T> {

    AllItems: Array<any> = []
    Selected: Array<any> = []
    get Single() {
        return this.Selected?.length === 1 ? this.Selected[0] : null;
    }

    get IsFirst() {
        if (!this.Single) {
            return null;
        }

        return this.AllItems[0].Id === this.Single.Id
    }

    get IsLast() {
        if (!this.Single) {
            return null;
        }

        return this.AllItems[this.AllItems.length - 1].Id === this.Single.Id
    }

}
