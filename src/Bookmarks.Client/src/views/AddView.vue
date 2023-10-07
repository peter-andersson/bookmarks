<script setup lang="ts">
  import router from '@/router';
  import {ref } from "vue";
  import type { Ref } from 'vue'
  import type { Bookmark } from "@/models";

  let adding : Ref<boolean> = ref(false);
  let error : Ref<string | null> = ref(null);
  let bookmark : Ref<Bookmark> = ref({
    "id": 0,
    "url": "",
    "tags": [],
    "title": null,
    "description": null
  });

  function cancel() {
    router.push('/');
  }

  // TODO: Load info from url
  // TODO: Autocomplete on tag names?

  async function add() {
    adding.value = true;

    try {
      const response = await fetch('/api/bookmark/', {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(bookmark.value)
      });
      if (response.ok) {
        await router.push('/');
      } else {
        adding.value = false;
        error.value = "API responded with " + response.statusText;
      }
    } catch (err : any) {
      error.value = err.toString();
    }
  }
</script>

<template>
  <h1>Add bookmark</h1>

    <div class="mb-3">
      <label for="url" class="form-label">URL</label>
      <input type="text" class="form-control" id="url" v-model="bookmark.url">
    </div>
  <div class="mb-3">
    <label for="tags" class="form-label">Tags</label>
    <input type="text" class="form-control" id="tags" v-model="bookmark.tags">
    <div class="form-text">Enter tags separated by space.</div>
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

  <button type="button" class="btn btn-success" @click="add" v-bind:disabled="adding">
    <span v-if="adding" class="spinner-border spinner-border-sm" aria-hidden="true"></span>
    <span v-if="adding" role="status">Adding...</span>
    <span v-if="!adding">Add</span>
  </button>
  <button type="button" class="btn btn-secondary ms-2" @click="cancel">Cancel</button>
</template>
