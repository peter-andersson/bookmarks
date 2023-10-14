<script setup lang="ts">
import type { BookmarkModel, Website } from "@/models";
import { api } from "@/services/api";
import { ref } from 'vue'
import type { Ref } from 'vue'

const props = defineProps<{
  bookmark: BookmarkModel | null,
  progress: boolean,
  buttonText: string
}>();

let bookmark : Ref<BookmarkModel> = ref({
  "id": 0,
  "url": "",
  "tags": [],
  "title": null,
  "description": null,
});

if (props.bookmark) {
  bookmark.value = props.bookmark;
}

let tags : Ref<string> = ref("");
bookmark.value.tags.forEach((tag) => {
  if (tags.value === "") {
    tags.value = tag;
  }
  else {
    tags.value = tags.value + " " + tag;
  }
});

const emit = defineEmits<{
  (e: "ok", bookmark: BookmarkModel): void
  (e: "cancel"): void
}>();

async function loadInfo() {
  if (!bookmark.value.url) {
    return;
  }

  if (bookmark.value.title || bookmark.value.description) {
    // Already has info, don't load new info
    return;
  }

  const info : Website | null = await api.loadInfo(bookmark.value.url);
  if (info) {
    bookmark.value.title = info.title;
    bookmark.value.description = info.description;
  }
}

let filteredTags = ref();
function searchTag() {
  const currentSearch : string = tags.value.split(" ").pop() ?? "";

  if (currentSearch === "") {
    filteredTags.value = "";
    return;
  }
  filteredTags.value = tagNames.filter((name) => {
    return name.toLowerCase().startsWith(currentSearch.toLowerCase());
  });
}

function ok() {
  let model  : BookmarkModel = {
    "id": bookmark.value.id,
    "url": bookmark.value.url,
    "title": bookmark.value.title,
    "description": bookmark.value.description,
    "tags": []
  };
  const tagNames = tags.value.split(" ");
  console.log(tagNames);
  tagNames.forEach((name) => {
    model.tags.push(name);
  });

  emit("ok", model);
}

function cancel() {
  emit("cancel");
}

let tagNames: string[] = [];
async function getTags() {
  tagNames = await api.getTags();
}

getTags();

</script>

<template>
  <div class="mb-3">
    <label for="url" class="form-label">URL</label>
    <input type="text" class="form-control" id="url" v-model="bookmark.url" @input="loadInfo">
  </div>
  <div class="mb-3">
    <div class="row">
      <div class="col-12 col-md-6">
        <label for="tags" class="form-label">Tags</label>
        <input id="tags" v-model="tags" class="form-control" @input="searchTag">
        <div class="form-text">Enter tags separated by space.</div>
      </div>
      <div class="col-12 col-md-6">
        <label for="tags" class="form-label">Tag suggestions</label>
        <textarea rows="5" class="form-control" v-model="filteredTags" readonly></textarea>
      </div>
    </div>
  </div>

  <div class="mb-3">
    <label for="title" class="form-label">Title</label>
    <input type="text" class="form-control" id="title" v-model="bookmark.title">
    <div class="form-text">Optional, load from website.</div>
  </div>
  <div class="mb-3">
    <label for="description" class="form-label">Description</label>
    <input type="text" class="form-control" id="description" v-model="bookmark.description">
    <div class="form-text">Optional, load from website.</div>
  </div>

  <button type="button" class="btn btn-success" @click="ok" v-bind:disabled="progress">
    <span v-if="progress" class="spinner-border spinner-border-sm" aria-hidden="true"></span>
    <span v-if="!progress">{{ buttonText }}</span>
  </button>
  <button type="button" class="btn btn-secondary ms-2" @click="cancel">Cancel</button>
</template>

<style scoped>

</style>